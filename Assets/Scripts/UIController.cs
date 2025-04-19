using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI Tittle;
    public TextMeshProUGUI description;
    public TextMeshProUGUI rating;

    // Reference to your database manager/controller
    // private DatabaseController database;

    void Start()
    {
        // Initialize database connection
        // database = GetComponent<DatabaseController>();
    }

    // Method to update UI with movie information
    /*
    public void UpdateMovieUI(MovieData movieData)
    {
        // Update text fields with movie information from database
        Tittle.text = movieData.title;
        description.text = movieData.description;
        rating.text = movieData.rating.ToString();

        // Update panel sprite with movie poster/image
        // Assuming movieData.posterSprite is a Sprite loaded from the database
        panel.sprite = movieData.posterSprite;
    }
    */

    /*
    // Example MovieData structure (to be replaced with your actual database structure)
    private class MovieData
    {
        public string title;
        public string description;
        public float rating;
        public Sprite posterSprite;
    }
    */
}
