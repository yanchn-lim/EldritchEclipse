using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Probability
{
    /* Selects a weighted item from the provided dictionary of items.
     * The key is the item and value is the weight of the item
     * Firstly, the weight of all the items are added up to get the total weight
     * Then, it goes through a while loop until an item has been acquired
     * To get an item, it goes through a for-loop where a random float is generated from
     * 0 to the total weight of all the items.
     * The random float is then compared to the current iteration's item and if the 
     * random float is less than the current iteration item's weight, it will return
     * this item.
     * If the random float is higher than this iteration's item weight, it will go to the
     * next iteration and compare again until it reaches the end of the dictionary.
    */
    public static T SelectWeightedItem<T>(Dictionary<T, float> weightedItems)
    {
        //THERE IS A FLAW WITH THIS
        //NUMBERS ARE NOT ACTUALLY ADDING UP PROPERLY

        float totalWeight = 0f;
        bool itemAcquired = false;

        // Calculate the total weight of all items in the dictionary.
        foreach (float weight in weightedItems.Values)
        {
            totalWeight += weight;
        }

        // Loop until an item is acquired
        while (!itemAcquired)
        {
            // Iterate through each item in the dictionary.
            foreach (var item in weightedItems)
            {
                // Generate a random value within the total weight range.
                float randomValue = Random.Range(0f, totalWeight);
                float currentWeight = item.Value;

                // Check if the random value falls within the current item's weight range.
                if (randomValue < currentWeight)
                {
                    return item.Key;
                }

            }
        }

        Debug.Log("returning default");
        return default;// Return the default value (null for reference types).
    }
}
