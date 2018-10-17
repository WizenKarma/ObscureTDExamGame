/*
    This is the player controller
    It contains all you'd expect to move the player
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControllerTutorial: MonoBehaviour
{
    private Rigidbody rb;

    // move, jump stuff
    #region Jump Variables
    [SerializeField] private float moveSpeed = 12f;
    [SerializeField] private float runSpeed = 20f;
    [SerializeField] private float jumpForce = 5.0f;
    private float intialMoveSpeed;
    #endregion
    // variable control
    #region Movement Variables
    private KeyCode moveLeft = KeyCode.A;
    private KeyCode moveRight = KeyCode.D;
    private KeyCode moveForward = KeyCode.W;
    private KeyCode moveBackward = KeyCode.S;
    private KeyCode jump = KeyCode.Space;
    private KeyCode sprintKey = KeyCode.LeftShift;
    private Vector3 moveDirection;
    private float moveHorizontal;
    private float moveVertical;
    #endregion
    // raycast for ground stuff
    #region Raycast Variables
    public LayerMask groundedLayer;
    [SerializeField] private float groundRayLength = 0.7f; //length of ray going to ground
    [SerializeField] private bool isGrounded = false;
    #endregion
    // third person camera stuff
    #region Camera Settings
    [SerializeField] private float distanceOffset = 5.0f;
    [SerializeField] private float heightOffset = 5.0f;
    [SerializeField] private float widthOffset = 5.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    //private float rbY = 0.0f;
    private float camY;
    private Camera cam;
    private Transform camTransform;
    [SerializeField] private float MAX_Y = 30f;
    [SerializeField] private float MIN_Y = -5f;
    private GameObject pivot;
    #endregion
    // UI visualisation things
    #region Widgets and Rays
    [SerializeField] private float cameraRayLength = 1f;
    [SerializeField] private GameObject crosshair;
    #endregion
    // Beginning the implementation of the objects
    [SerializeField] private bool canBuild;
    private itemPlacer ip;

    public GameManager gameManager;

    private grid placementGrid;
    public GameObject preview;
    private GameObject previewTower;
    public GameObject wall;
    private List<Button> spawnButtons = new List<Button>();
    public Button BuildButtonPrefab;
    public RectTransform BuildButtonParent;
    private bool isBuilding;
    private GameObject SelectedTower;
    private bool towersDrafted;
    private bool combinesRecognized;
    #region Tower Variables
    [SerializeField] private SpawnedTowers Inventory; // the overall inventory
    [SerializeField] private SpawnedTowers RoundInventory; // the round inventory
    [System.Serializable]
    public struct Possible
    {
        public Tower tower;
        public int weight;//higher weight means more frequent
        public float occurence;
    }

    public List<Combiner> recipes = new List<Combiner>();
    public List<Possible> towers = new List<Possible>();
    public List<Tower> towersToPlaceThisRound = new List<Tower>();

    private Tower TowerToBuild;
    private Tower[] TowersOnButtons = new Tower[5];
    private Tower[] CombinationButtons = new Tower[5];
    float totalWeight;

    private int towerAtIndex;
    #endregion

    /// <summary>
    ///  Testing this value
    /// </summary>
    public int towerCounter = 0;

    private void OnValidate()
    {
        placementGrid = FindObjectOfType<grid>();
        totalWeight = 0;
        towersDrafted = false;
        foreach (Possible t in towers)
            totalWeight += t.weight;
        Inventory = this.GetComponent<SpawnedTowers>();
    }

    // Use this for initialization
    void Start()
    {
        OnValidate();
        Cursor.lockState = CursorLockMode.Locked;
        canBuild = false;
        intialMoveSpeed = moveSpeed;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationY;

        cam = Camera.main;
        camTransform = cam.transform;
        pivot = GameObject.FindGameObjectWithTag("pivot");
        ip = FindObjectOfType<itemPlacer>();
        crosshair = GameObject.Find("Cross Hair");

        //pickRandomTowers(5);
        isBuilding = true;
        combinesRecognized = false;
    }


    void ShowPreview()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength))
        {
            if (hitInfo.collider.tag == "Ground")
            {
                preview.transform.position = placementGrid.GetNearestPointOnGrid(hitInfo.point) - Vector3.up * 0.45f;
                if (previewTower != null)
                    previewTower.transform.position = placementGrid.GetNearestPointOnGrid(hitInfo.point) + Vector3.up;
            }
        }
    }


    #region Tower Functions
    void pickRandomTowers(int count) //populates the towersToPlaceThisRound variable with 5 towers
    {
        for (int i = 0; i < count; i++)
        {
            // Generate a random position in the list.
            float pick = Random.value * totalWeight;
            int chosenIndex = 0;
            float cumulativeWeight = towers[0].weight;
            // Step through the list until we've accumulated more weight than this.
            // The length check is for safety in case rounding errors accumulate.
            while (pick > cumulativeWeight && chosenIndex < towers.Count - 1)
            {
                chosenIndex++;
                cumulativeWeight += towers[chosenIndex].weight;
            }
            towersToPlaceThisRound.Add(towers[chosenIndex].tower);
        }
        draftButtons();
        towersDrafted = true;
    }

    void draftButtons()
    {
        foreach (Tower t in towersToPlaceThisRound)
        {
            Button btn = Instantiate(BuildButtonPrefab, BuildButtonParent);
            btn.GetComponentInChildren<Image>().sprite = t.preview;
            btn.GetComponentInChildren<Text>().text = t.name;
            spawnButtons.Add(btn);
        }
    }

    private Tower SelectTower(bool canBuild, int keyPressed)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength))
        {
            //if (groundedLayer == (groundedLayer | (1 << hitInfo.collider.gameObject.layer)))
            //  {
            ToggleCrossHair(true);
            towerAtIndex = keyPressed;
            if (previewTower != null)
            {
                Destroy(previewTower);
            }
            if (towersToPlaceThisRound[keyPressed].prefab != null)
                previewTower = Instantiate(towersToPlaceThisRound[keyPressed].prefab, Vector3.down * 10, Quaternion.identity);
            return towersToPlaceThisRound[keyPressed];
            // }
        }
        else
        {
            ToggleCrossHair(false);
            return null;
        }
    }

    bool build(Vector3 pos, Tower towerToBuild)
    {
        if (towerToBuild != null)
        {
            GameObject tow = Instantiate(towerToBuild.prefab, pos, Quaternion.identity); //where the tower is instantiated
            tow.name = towerToBuild.name; //fix up the name for checking later
            Instantiate(wall, pos - Vector3.up * 0.5f, Quaternion.identity);
            Tower SOTower = Instantiate(towerToBuild);
            SOTower.TargetTower = tow;
            SOTower.name = towerToBuild.name;
            tow.GetComponent<InGameTower>().tower = SOTower;
            RoundInventory.AddTower(tow.GetComponent<InGameTower>().tower, tow);
            towersToPlaceThisRound[towerAtIndex] = new Tower();
            TowerToBuild = null;
            spawnButtons[towerAtIndex].interactable = false;
            spawnButtons[towerAtIndex].GetComponentInChildren<Image>().sprite = null;
            return true;
        }
        return false;
    }

    void selectTower()
    {
        if (RoundInventory.CountTowers() >= 5)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength))
            {
                if (hitInfo.collider.tag == "Tower")
                {
                    Inventory.AddTowers(RoundInventory.SaveTower(hitInfo.collider.gameObject));
                }
            }
        }
    }

    /// <summary>
    /// this action should only be possible after all 5 towers have been built.
    /// </summary>
    void ViewTower()
    {
        RaycastHit hitInfo;
        int index = 1;
        if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength))
        {
            if (hitInfo.collider.tag == "Tower")
            {
                SelectedTower = hitInfo.collider.gameObject;
                RoundInventory.InsertAtFirst(SelectedTower);
                foreach (Button b in spawnButtons)
                {
                    Destroy(b.gameObject);
                }
                spawnButtons.Clear();
                Button button = Instantiate(BuildButtonPrefab, BuildButtonParent);
                button.GetComponentInChildren<Image>().sprite = hitInfo.collider.GetComponent<InGameTower>().tower.preview;
                button.GetComponentInChildren<Text>().text = "Keep";
                spawnButtons.Add(button);
                TowersOnButtons[0] = hitInfo.collider.GetComponent<InGameTower>().tower;
                foreach (Combiner c in recipes)
                {
                    if (c.CanCraft(RoundInventory))
                    {
                        foreach (NumberOfTowers comp in c.components)
                        {
                            if (comp.Tower.name == hitInfo.collider.name)
                            {
                                index++;
                                TowersOnButtons[index] = c.output[0].Tower;
                                //print("This tower is contained in the recipe for " + c.name);
                                Button btn = Instantiate(BuildButtonPrefab, BuildButtonParent);
                                btn.GetComponentInChildren<Image>().sprite = c.output[0].Tower.preview;
                                btn.GetComponentInChildren<Text>().text = c.output[0].Tower.name;
                                spawnButtons.Add(btn);
                            }
                        }
                    }
                }
                for (int i = spawnButtons.Count; i < 5; i++)
                {
                    Button btn = Instantiate(BuildButtonPrefab, BuildButtonParent);
                    btn.interactable = false;
                    spawnButtons.Add(btn);
                }
            }
        }
    }

    void SelectDraftTowers()
    { //Means that the player can now select towers to build
        for (int i = 1; i <= 5; ++i)
        {
            if (Input.GetKeyDown("" + i))
            {
                TowerToBuild = SelectTower(true, i - 1); // loads the currently selected tower as the one that will now be built
                canBuild = true;
            }
        }
    }

    void SelectCombineTowers()
    {
        if (Input.GetKeyDown("" + 1))
        {
            selectTower();
            gameManager.ChangeBehaviour();
        }
        for (int i = 2; i <= 5; ++i)
        {
            if (Input.GetKeyDown("" + i))
            {
                TowerToBuild = TowersOnButtons[i];
                foreach (Combiner c in recipes)
                {
                    if (c.output[0].Tower.name == TowerToBuild.name)
                    {
                        c.Craft(RoundInventory); //crafts the tower, (which only means adding it to the inventory, it must be instantiated too
                        GameObject tow = Instantiate(TowerToBuild.prefab, SelectedTower.transform.position, Quaternion.identity); //where the tower is instantiated
                        tow.name = TowerToBuild.name; //fix up the name for checking later
                        Instantiate(wall, SelectedTower.transform.position - Vector3.up * 0.5f, Quaternion.identity);
                        Tower SOTower = Instantiate(TowerToBuild);
                        SOTower.name = TowerToBuild.name;
                        SOTower.TargetTower = tow;
                        SOTower.name = TowerToBuild.name;
                        tow.GetComponent<InGameTower>().tower = SOTower;
                        RoundInventory.AddTower(tow.GetComponent<InGameTower>().tower, tow);
                        towersToPlaceThisRound[towerAtIndex] = new Tower();
                        spawnButtons[towerAtIndex].interactable = false;
                        spawnButtons[towerAtIndex].GetComponentInChildren<Image>().sprite = null;
                        Inventory.AddTowers(RoundInventory.SaveTower(tow.gameObject));
                    }
                }
            }
        }

    }


    #endregion

    #region Utility Functions for placement etc.
    public Vector3 PlaceCubeNear(Vector3 clickPoint)
    {
        Vector3 finalPosition = placementGrid.GetNearestPointOnGrid(clickPoint);
        return finalPosition;
    }
    #endregion

    /// <summary>
    /// Controls the various ways the player can input control
    /// </summary>
    private void PlayerInput()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
            gameManager.currentPhase = PhaseBuilder.PhaseType.Combine;
        Cursor.visible = false;
        switch (gameManager.currentPhase)
        {
            case (PhaseBuilder.PhaseType.Tutorial1):
                {
                    break;
                }

            case (PhaseBuilder.PhaseType.Tutorial2):
                {
                    ShowPreview(); //shows the little preview tower, must replace with the hologram version
                    PlayerRotation();
                    break;
                }

            case (PhaseBuilder.PhaseType.Tutorial3):
                {
                    if (!towersDrafted)
                    {
                        print("Towers have been drafted");
                        pickRandomTowers(5);
                    }
                    SelectDraftTowers();
                    ShowPreview(); //shows the little preview tower, must replace with the hologram version
                    PlayerRotation();
                    break;
                }

            case (PhaseBuilder.PhaseType.Tutorial4):
                {
                    SelectDraftTowers();
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (build(preview.transform.position + Vector3.up, TowerToBuild))
                        {
                            if (previewTower != null)
                            {
                                Destroy(previewTower);
                            }
                            canBuild = false;
                            //towerCounter++; /// Tower counter for GM to change to the COmbine phase /////////////////
                        }
                    }
                    ShowPreview(); //shows the little preview tower, must replace with the hologram version
                    PlayerRotation();
                    break;
                }

            case (PhaseBuilder.PhaseType.Tutorial5):
                {

                    ShowPreview(); //shows the little preview tower, must replace with the hologram version
                    PlayerRotation();

                    if (Input.GetMouseButtonDown(0))
                    {
                        ViewTower();
                    }
                    SelectCombineTowers();
                    break;
                }
        }
    }

    public void setBuildTowers(Tower towerToAdd, int index)
    {
        TowersOnButtons[index] = towerToAdd;
    }


    #region Building Aux Functions
    public void SetBuild(bool boolean)
    {
        if (boolean)
        {
            canBuild = true;
        }
        else
        {
            canBuild = false;
        }
    }

    public int GetTowerCount()
    {
        return towerCounter;
    }

    public void ResetTowerCount()
    {
        // need to discuss how it works
        //towersDrafted = false;
        //canBuild = true;
        //isBuilding = false;
        towerCounter = 0;
    }
    #endregion

    // All of the player movement functions are contained within this
    #region PlayerMovement
    private void PlayerRotation()
    {
        mouseX += Input.GetAxis("Mouse X");
        mouseY -= Input.GetAxis("Mouse Y");
        mouseY = Mathf.Clamp(mouseY, MIN_Y, MAX_Y);
        Quaternion rotation = Quaternion.Euler(0, mouseX, 0);
        transform.rotation = rotation;
    }

    // Needs commenting and cleaning up*
    private void CameraRotation()
    {
        Vector3 cameraNewRot = new Vector3(widthOffset, heightOffset, -distanceOffset);
        //Vector3 cameraNewPos = new Vector3(widthOffset, heightOffset, -distanceOffset);
        Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
        cam.transform.position = pivot.transform.position + rotation * cameraNewRot;
        cam.transform.forward = pivot.transform.forward;
        cam.transform.LookAt(pivot.transform);
        pivot.transform.forward = transform.forward;
        Debug.DrawRay(cam.transform.position, cam.transform.forward * cameraRayLength, Color.red);
    }

    /// <summary>
    /// Summary: All behaviour for movement
    /// </summary>
    /// <param name="isGrounded"></param>
    private void Movement(bool isGrounded)
    {
        // cut movement if theyre in air
        if (isGrounded)
        {

            // basic movement
            if (Input.GetKey(moveForward))
            {
                rb.velocity = transform.forward * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(moveLeft))
            {
                rb.velocity = -transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(moveRight))
            {
                rb.velocity = transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(moveBackward))
            {
                rb.velocity = -transform.forward * moveSpeed * Time.deltaTime;
            }
            // forcing zero movement if no input to fix ice skating
            else if (Input.GetKeyUp(moveForward) | Input.GetKeyUp(moveLeft) | Input.GetKeyUp(moveRight) | Input.GetKeyUp(moveBackward))
            {
                rb.velocity = new Vector3(0, 0, 0);
            }


            // running

            if (Input.GetKey(moveForward) && Input.GetKey(sprintKey))
            {
                moveSpeed = runSpeed;
            }
            else
            {
                moveSpeed = intialMoveSpeed;
            }



        }
    }

    private void Jump()
    {
        if (Input.GetKey(jump) && isGrounded)
        {
            rb.velocity = new Vector3(rb.velocity.x, 1 * Time.deltaTime * jumpForce, rb.velocity.z);
        }
    }

    private bool CheckGround()
    {
        Debug.DrawRay(transform.position - new Vector3(0, 0.5f, 0), -transform.up * groundRayLength, Color.cyan);
        if (Physics.Raycast(transform.position - new Vector3(0, 0.5f, 0), -transform.up, groundRayLength, groundedLayer))
        {
            return true;
        }
        return false;
    }

    private void ToggleCrossHair(bool inRange)
    {
        if (inRange)
        {
            crosshair.SetActive(inRange);
        }
        else
        {
            crosshair.SetActive(inRange);
        }
    }
    #endregion

    // Untested things
    #region Unimplemented Things
    /*private void StrafeMovement(bool isGrounded)
    {   
        // cut movement if theyre in air
        if (isGrounded)
        {
            // allows for strafe
            if (Input.GetKey(moveLeft) && Input.GetKey(moveForward))
            {
                rb.velocity = new Vector3(-1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime, rb.velocity.y, 1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime);
            }
            if (Input.GetKey(moveRight) && Input.GetKey(moveForward))
            {
                rb.velocity = new Vector3(1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime, rb.velocity.y, 1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime);
            }
            if (Input.GetKey(moveLeft) && Input.GetKey(moveBackward))
            {
                rb.velocity = new Vector3(-1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime, rb.velocity.y, -1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime);
            }
            if (Input.GetKey(moveRight) && Input.GetKey(moveBackward))
            {
                rb.velocity = new Vector3(1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime, rb.velocity.y, -1 * (moveSpeed - moveSpeed / 2) * Time.deltaTime);
            }
        }
    }*/
    #endregion

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        Movement(isGrounded);
        Jump();
        isGrounded = CheckGround();
        // quick test in build
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            selectTower();

        }
    }

    void FixedUpdate()
    {


    }

    void LateUpdate()
    {
        CameraRotation();
    }
}
