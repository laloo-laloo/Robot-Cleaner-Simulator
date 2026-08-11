using UnityEngine;

public class UpgradeUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenUpgradeUI()
    {
        gameObject.SetActive(true);
    }
    public void CloseUpgradeUI()
    {
        gameObject.SetActive(false);
    }
}
