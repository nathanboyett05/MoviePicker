using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image panel;
    public TextMeshProUGUI Tittle;
    public TextMeshProUGUI description;
    public TextMeshProUGUI rating;

    private TMDBController tmdbController;
    private Vector2 touchStart;
    private bool isDragging = false;
    private float swipeThreshold = 50f;
    private RectTransform cardTransform;

    void Start()
    {
        tmdbController = GetComponent<TMDBController>();
        cardTransform = panel.GetComponent<RectTransform>();
    }

    void Update()
    {
        // Handle mouse input for Unity Editor and PC
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - touchStart;
            
            // Rotate and move the card based on swipe
            float rotation = swipeDelta.x * 0.1f;
            cardTransform.rotation = Quaternion.Euler(0, 0, -rotation);
            cardTransform.position = new Vector3(
                transform.position.x + swipeDelta.x * 0.5f,
                transform.position.y,
                transform.position.z
            );
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector2 swipeDelta = (Vector2)Input.mousePosition - touchStart;
            
            if (Mathf.Abs(swipeDelta.x) > swipeThreshold)
            {
                bool liked = swipeDelta.x > 0;
                OnSwipeComplete(liked);
            }
            else
            {
                // Reset position and rotation if swipe wasn't strong enough
                ResetCard();
            }
            
            isDragging = false;
        }
    }

    private void ResetCard()
    {
        cardTransform.rotation = Quaternion.identity;
        cardTransform.position = transform.position;
    }

    private void OnSwipeComplete(bool liked)
    {
        // For MVP, just load next movie regardless of like/dislike
        tmdbController.LoadNextMovie();
        ResetCard();
    }

    public void UpdateMovieUI(MovieData movieData, Sprite posterSprite)
    {
        Tittle.text = movieData.title;
        description.text = movieData.overview;
        rating.text = movieData.vote_average.ToString("F1") + "/10";
        panel.sprite = posterSprite;
    }
}
