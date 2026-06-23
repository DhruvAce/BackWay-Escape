using UnityEngine;

public class PlayerColorLoader : MonoBehaviour
{
    public Renderer body;

    public GameObject[] hats;

    public Material[] faceMaterials;

    void Awake()
    {
        LoadBodyColor();
        LoadHat();
        LoadFace();
    }

    void LoadBodyColor()
    {
        float r = PlayerPrefs.GetFloat("PlayerColorR",1f);
        float g = PlayerPrefs.GetFloat("PlayerColorG",1f);
        float b = PlayerPrefs.GetFloat("PlayerColorB",1f);

        Material[] mats = body.sharedMaterials;

        mats[0].color = new Color(r,g,b);

        body.sharedMaterials = mats;
    }

    void LoadHat()
    {
        int hatIndex = PlayerPrefs.GetInt("HatIndex",0);

        for(int i=0;i<hats.Length;i++)
        {
            if(hats[i] != null)
                hats[i].SetActive(i == hatIndex);
        }
    }

    void LoadFace()
    {
        int faceIndex = PlayerPrefs.GetInt("FaceIndex",0);

        Material[] mats = body.sharedMaterials;

        if(faceIndex >= 0 && faceIndex < faceMaterials.Length)
        {
            mats[1] = faceMaterials[faceIndex];
        }

        body.sharedMaterials = mats;
    }
}