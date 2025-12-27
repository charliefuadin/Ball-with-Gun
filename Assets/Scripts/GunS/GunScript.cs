using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScript : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;
    private int selectedGun = 0;

    private TargetFollow targetFollow;

    private void Start()
    {
        GunReset();
        guns[0].gameObject.SetActive(true);
        targetFollow = GetComponent<TargetFollow>();
    }
    private void Update()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        targetFollow.aimAtTarget(mousePosition);
        GunSwitch();
    }


    private void GunSwitch()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            //Resets all guns and activated current index
            GunReset();
            selectedGun += 1;
            if (selectedGun >= guns.Length)
            {
                selectedGun = 0;
                Debug.Log("Equals zero");
            }
            Debug.Log(selectedGun);
            guns[selectedGun].gameObject.SetActive(true);
        }
    }
    
    private void GunReset()
    {
        //Makes sure all the guns are reset
        for (int i = 0; i < guns.Length; i++)
        {
            guns[i].SetActive(false);
        }
    }

}

    

  
