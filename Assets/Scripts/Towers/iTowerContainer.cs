using UnityEngine;

public interface iTowerController {
    //bool ContainsTower(Tower tower);
    int TowerCount(string towerID);
    Tower RemoveTower(string towerID);
    bool RemoveTower(Tower tower);
    bool AddTower();
    bool AddTower(Tower tower,GameObject gameObj);
    bool AddTower(Tower tower);
    bool IsFull();
    void Clear();
    Tower KillTower(string towerID);
}