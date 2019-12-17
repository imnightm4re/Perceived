﻿/// DarkTreeDevelopment (2019) DarkTree FPS v1.2
/// If you have any questions feel free to write me at email --- darktreedevelopment@gmail.com ---
/// Thanks for purchasing my asset!

using UnityEngine;
using UnityEngine.UI;

namespace DarkTreeFPS
{
    public class UseObjects : MonoBehaviour
    {
        [Tooltip("The distance within which you can pick up item")]
        public float distance = 1.5f;
        
        private GameObject use;
        private GameObject useCursor;
        private Text useText;

        private Inventory inventory;

        private Button useButton;

        private void Start()
        {
            useCursor = GameObject.Find("UseCursor");
            useText = useCursor.GetComponentInChildren<Text>();
            useCursor.SetActive(false);

            inventory = FindObjectOfType<Inventory>();
            
        }

        void Update()
        {
            Pickup();
        }

        public void Pickup()
        {
            RaycastHit hit;

            //Hit an object within pickup distance
            if (Physics.Raycast(transform.position, transform.forward, out hit, distance))
            {
                if (hit.collider.tag == "Item")
                {
                    //Get an item which we want to pickup
                    use = hit.collider.gameObject;
                    useCursor.SetActive(true);
                    

                    if (use.GetComponent<Item2>())
                    {
                        useText.text = use.GetComponent<Item2>().title;
                            if (Input.GetKeyDown(null))
                            {
                                inventory.GiveItem(use.GetComponent<Item2>());
                                use = null;
                            }
                        
                            
                    }
                    //useText.text = use.weaponNameToAddAmmo + " Ammo x " + use.ammoQuantity;
                    else if (use.GetComponent<WeaponPickup>()) {

                        useText.text = use.GetComponent<WeaponPickup>().weaponNameToEquip;
                        
                            if (Input.GetKeyDown(null))
                            {
                                use.GetComponent<WeaponPickup>().Pickup();
                            }
                        
                    }
                }
                else
                {
                    //Clear use object if there is no an object with "Item" tag
                    use = null;
                    useCursor.SetActive(false);
                    

                    useText.text = "";
                }
            }
            else
            {
                useCursor.SetActive(false);
                

                useText.text = "";
            }
        }
    }
}
