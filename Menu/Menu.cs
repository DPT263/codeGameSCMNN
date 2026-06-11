using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public void BatDau()
    {
        SceneManager.LoadScene("KhoiDau");
    }
    public void Thoat()
    {
        Application.Quit();
    }
}
