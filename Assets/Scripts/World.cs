using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    public int maxPopulation1;
    public int maxPopulation2;
    public int numPopulation1 = 0;
    public int numPopulation2 = 0;
    public int ownedTerritory1 = 1;
    public int ownedTerritory2 = 1;

    // Start is called before the first frame update
    void Start()
    {
        maxPopulation1 = ownedTerritory1 * 3;
        maxPopulation2 = ownedTerritory2 * 5;

    }

    // Update is called once per frame
    void Update()
    {
        maxPopulation1 = ownedTerritory1 * 3;
        maxPopulation2 = ownedTerritory2 * 5;
    }
}
