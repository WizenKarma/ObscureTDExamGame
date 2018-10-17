using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    public float moveSpeed = 220f;   
    private Rigidbody myRb;

    #region CAMERA_VARS
    private Camera cam;
    private Transform camTransform;
    private float MIN_Y = -90.0f;
    private float MAX_Y = 90.0f;
    #endregion

    #region UI_VISUAl_VARS
    [SerializeField] private float cameraRayLength = 1f;
    [SerializeField] private GameObject crosshair;
    #endregion

    #region TOWER_VARS
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
    public int towerCounter = 0;
    #endregion

    #region ROTATION_VARS
    public float mouse_Sensitivity = 50.0f;
    private float mouseX = 0.0f;
    private float mouseY = 0.0f;
    private GameObject pivot;
    private Vector3 oneZeroOne;
    #endregion

    #region TOP_DOWN_VARS
    public float snapSpeed = 10f;
    public GameObject posToSnapTo;
    public bool isTopDown;
    private Transform previousTransform;
    private Vector3 previousPos;
    public float snapTime = 10f;
    public GameObject lookAtTarget;
    private Vector3 snapVector = Vector3.zero;
    #endregion

    #region CAMERA_MOVEMENT_ROTATION
    private void CameraRotation()
    {
        Cursor.lockState = CursorLockMode.Locked;
        if (!isTopDown)
        {
            mouseX += Input.GetAxisRaw("Mouse X");
            mouseY -= Input.GetAxisRaw("Mouse Y");
            float angle = Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg;
            mouseY = Mathf.Clamp(mouseY, MIN_Y, MAX_Y);
            Quaternion rotation = Quaternion.Euler(mouseY, mouseX, 0);
            transform.rotation = rotation;
            transform.forward = pivot.transform.forward;
            transform.LookAt(pivot.transform);
            pivot.transform.forward = transform.forward;
        }
    }

    private void CameraMovement()
    {
        #region KILL_MOVEMENT
        if (Input.GetKeyUp(KeyCode.W) |
            Input.GetKeyUp(KeyCode.A) |
            Input.GetKeyUp(KeyCode.D) |
            Input.GetKeyUp(KeyCode.S) |
            Input.GetKeyUp(KeyCode.Space) |
            Input.GetKeyUp(KeyCode.LeftShift))
        {
            myRb.velocity = Vector3.zero;
        }
        #endregion

        #region SPECTATE_MOVEMENT
        if (!isTopDown)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                myRb.velocity = new Vector2(0, 1 * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                myRb.velocity = new Vector2(0, -1 * moveSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.W))
            {
                myRb.velocity = transform.forward * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                myRb.velocity = -transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                myRb.velocity = transform.right * moveSpeed * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                myRb.velocity = -transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        #endregion

        #region TOP_DOWN_MOVEMENT
        if (isTopDown)
        {
            Vector3 input = new Vector3();
            input.x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
            input.z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
            myRb.velocity = input;
        }
        #endregion
    }
    #endregion

    #region TOP_DOWN_FUNCTIONALITY 

    private void ToggleTopDown()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(!isTopDown)
            {
                SnapToTopDown();
            }
            else
            {
                ReturnToSpectate();
            }
        }
    }

    private void SnapToTopDown()
    {
        isTopDown = true;
        previousPos = transform.position;
        previousTransform = transform;
        snapVector = new Vector3(90f, 0, 0);
        transform.position = Vector3.Slerp(transform.position, posToSnapTo.transform.position, snapSpeed);
        transform.eulerAngles = snapVector;
    }


    private void ReturnToSpectate()
    {
        isTopDown = false;
        transform.position = Vector3.Lerp(transform.position, previousPos, snapSpeed);
    }
    #endregion

    #region TOWER_FUCNTIONS
    public List<Combiner> possibleRecipes()
    {
        return recipes;
    }
    public List<Possible> possibleTowers()
    {
        return towers;
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
            /*   RaycastHit hitInfo;
               if (Physics.Raycast(camTransform.position, camTransform.forward, out hitInfo, cameraRayLength))
               {
                   if (hitInfo.collider.tag == "Tower")
                   {
                       Inventory.AddTowers(RoundInventory.SaveTower(hitInfo.collider.gameObject));
                   }
               }*/
            Inventory.AddTowers(RoundInventory.SaveTower(SelectedTower));
            RoundInventory.RemoveTower(SelectedTower.GetComponent<InGameTower>().tower.ID);
        }
    }

    /// <summary>
    /// this action should only be possible after all 5 towers have been built.
    /// </summary>
    void ViewTower()
    {
        bool isRoundTower = false;
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
                isRoundTower = RoundInventory.ContainsThisTower(hitInfo.collider.gameObject.GetComponent<InGameTower>().tower);
                if (isRoundTower)
                {
                    Button button = Instantiate(BuildButtonPrefab, BuildButtonParent);
                    button.GetComponentInChildren<Image>().sprite = hitInfo.collider.GetComponent<InGameTower>().tower.preview;
                    button.GetComponentInChildren<Text>().text = "Keep";
                    spawnButtons.Add(button);
                    TowersOnButtons[0] = hitInfo.collider.GetComponent<InGameTower>().tower;
                }
                else {
                    Button btn = Instantiate(BuildButtonPrefab, BuildButtonParent);
                    btn.interactable = false;
                    spawnButtons.Add(btn);
                }

                SpawnedTowers inventoryToUse;
                if (isRoundTower)
                    inventoryToUse = RoundInventory;
                else
                    inventoryToUse = Inventory;

                foreach (Combiner c in recipes)
                {
                    if (c.CanCraft(inventoryToUse))
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
                    if (c.output[0].Tower.ID == TowerToBuild.ID)
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

    public void setBuildTowers(Tower towerToAdd, int index)
    {
        TowersOnButtons[index] = towerToAdd;
    }

    public void TowerReset()
    {
        foreach (Button b in spawnButtons)
        {
            Destroy(b.gameObject);
        }
        spawnButtons.Clear();
        towersToPlaceThisRound.Clear();

        // reset
        //pickRandomTowers(5);
        isBuilding = true;
        combinesRecognized = false;
        canBuild = false;
        totalWeight = 0;
        towersDrafted = false;
        foreach (Possible t in towers)
            totalWeight += t.weight;
    }

    public void ResetTowerCount()
    {
        // need to discuss how it works
        // towersDrafted = false;
        // canBuild = true;
        // isBuilding = false;
        towerCounter = 0;
    }
    #endregion

    #region INPUT_FUNCTIONS
    void InputControl()
    {
        CameraRotation();
        CameraMovement();
        ToggleTopDown();
        TowerControl();
    }

    void TowerControl()
    {
        ShowPreview(); //shows the little preview tower, must replace with the hologram version
        if (Input.GetKeyDown(KeyCode.C))
            gameManager.currentPhase = PhaseBuilder.PhaseType.Combine;
        Cursor.visible = false;
        switch (gameManager.currentPhase)
        {
            case (PhaseBuilder.PhaseType.Build):
                {
                    if (!towersDrafted)
                    {
                        print("You can Draft");
                        pickRandomTowers(5);
                    }
                    else if (!canBuild)
                    {
                        SelectDraftTowers();
                        if (Input.GetMouseButtonDown(0))
                        {
                            ViewTower();

                        }
                    }
                    else if (canBuild)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            if (build(preview.transform.position + Vector3.up, TowerToBuild))
                            {
                                if (previewTower != null)
                                {
                                    Destroy(previewTower);
                                }
                                canBuild = false;
                                towerCounter++; /// Tower counter for GM to change to the COmbine phase /////////////////
                            }
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            if (previewTower != null)
                            {
                                Destroy(previewTower);
                            }
                            //draftButtons();
                            canBuild = false;
                        }
                    }
                    break;
                }
            case (PhaseBuilder.PhaseType.Combine):
                {
                    gameManager.newPhase = true; /// prevent premature phase changing
                    if (!combinesRecognized)
                    {
                        print("combining now");
                        foreach (Tower t in RoundInventory.GetAllTowers())
                        {
                            foreach (Combiner c in recipes)
                            {
                                if (c.CanCraft(RoundInventory))
                                {
                                    foreach (NumberOfTowers comp in c.components)
                                    {
                                        if (comp.Tower.ID == t.ID)
                                        {
                                            t.TargetTower.GetComponent<Light>().enabled = true;
                                        }
                                    }
                                }
                            }
                        }
                        combinesRecognized = true;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {
                        ViewTower();
                    }
                    SelectCombineTowers();
                    
                    break;
                }
            case (PhaseBuilder.PhaseType.Attack):
                {

                    break;
                }
        }

        if (Input.GetMouseButtonDown(1))
        {
            isBuilding = false;
        }

    }
    #endregion

    #region AUX_FUNCTIONS
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

    #region UNITY_FUNCTIONS
    private void OnValidate()
    {
        placementGrid = FindObjectOfType<grid>();
        totalWeight = 0;
        towersDrafted = false;
        foreach (Possible t in towers)
            totalWeight += t.weight;
        Inventory = this.GetComponent<SpawnedTowers>();
    }

    private void Awake()
    {
        previousTransform = transform;
        oneZeroOne = new Vector3(1, 1, 1);
        myRb = GetComponent<Rigidbody>();
        myRb.useGravity = false;
        pivot = GameObject.Find("pivot");
        cam = Camera.main;
        camTransform = cam.transform;
    }

    // reevaluate why we have both start and awake?
    private void Start()
    {
        OnValidate();
        ip = FindObjectOfType<itemPlacer>();
    }

    void Update()
    {
        InputControl();
    }
    #endregion
}
