using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class uiControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] healthPoints playerHealth;
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] Score score;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = playerHealth.HP.ToString();
        scoreText.text = score.points.ToString();
    }
}
