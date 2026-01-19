using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class TMDBController : MonoBehaviour
{
    private const string API_KEY = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiIxNjE0OWIyZWIwZDgyYjg5NDMzOTc5OTIwMzU3ZDZkMyIsIm5iZiI6MTc0NTc5MTY2OS4yMywic3ViIjoiNjgwZWFhYjU1YTliNzhkMjcyODEyODliIiwic2NvcGVzIjpbImFwaV9yZWFkIl0sInZlcnNpb24iOjF9.n1kBGIpzO5t2gI1IRmYHeEo_3nekmnoZfMuMPqYCfq8"; // Replace with actual API key
    private const string BASE_URL = "https://api.themoviedb.org/3";
    private const string IMAGE_BASE_URL = "https://image.tmdb.org/t/p/w500"; // w500 size for posters
    
    private UIController uiController;
    private List<MovieData> movieQueue = new List<MovieData>();
    private int currentMovieIndex = 0;

    void Start()
    {
        uiController = GetComponent<UIController>();
        if (uiController == null)
        {
            Debug.LogError("Failed to find UIController component on the same GameObject");
            return;
        }
        Debug.Log("TMDBController initialized successfully");
        StartCoroutine(GetPopularMovies());
    }

    public IEnumerator GetPopularMovies()
    {
        string url = $"{BASE_URL}/movie/popular";
        
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("Authorization", $"Bearer {API_KEY}");
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                string jsonResult = webRequest.downloadHandler.text;
                Debug.Log($"Received JSON: {jsonResult}");
                
                MovieListResponse response = JsonUtility.FromJson<MovieListResponse>(jsonResult);
                
                if (response == null)
                {
                    Debug.LogError("Failed to parse JSON response to MovieListResponse");
                    yield break;
                }
                
                if (response.results == null)
                {
                    Debug.LogError("MovieListResponse.results is null");
                    yield break;
                }
                
                if (response.results.Count == 0)
                {
                    Debug.LogError("MovieListResponse.results is empty");
                    yield break;
                }
                
                Debug.Log($"Successfully parsed {response.results.Count} movies");
                movieQueue = response.results;
                StartCoroutine(LoadCurrentMovie());
            }
            else
            {
                Debug.LogError($"Error fetching movies: {webRequest.error}");
                Debug.LogError($"Response Code: {webRequest.responseCode}");
                Debug.LogError($"Response Headers: {webRequest.GetResponseHeaders()}");
                if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    Debug.LogError($"Response Body: {webRequest.downloadHandler.text}");
                }
            }
        }
    }

    public IEnumerator LoadCurrentMovie()
    {
        if (currentMovieIndex >= movieQueue.Count)
        {
            // Reload more movies when we run out
            StartCoroutine(GetPopularMovies());
            yield break;
        }

        MovieData currentMovie = movieQueue[currentMovieIndex];
        
        // Start downloading the poster
        if (!string.IsNullOrEmpty(currentMovie.poster_path))
        {
            Debug.Log($"Loading poster from URL: {IMAGE_BASE_URL + currentMovie.poster_path}");
            string posterUrl = IMAGE_BASE_URL + currentMovie.poster_path;
            using (UnityWebRequest imageRequest = UnityWebRequestTexture.GetTexture(posterUrl))
            {
                yield return imageRequest.SendWebRequest();

                if (imageRequest.result == UnityWebRequest.Result.Success)
                {
                    Texture2D texture = ((DownloadHandlerTexture)imageRequest.downloadHandler).texture;
                    Sprite posterSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                    Debug.Log($"Successfully loaded poster for movie: {currentMovie.title}");
                    
                    // Update UI with movie data and poster
                    uiController.UpdateMovieUI(currentMovie, posterSprite);
                }
                else
                {
                    Debug.LogError($"Error loading poster: {imageRequest.error}");
                    Debug.LogError($"Response Code: {imageRequest.responseCode}");
                    Debug.LogError($"Response Headers: {imageRequest.GetResponseHeaders()}");
                    if (!string.IsNullOrEmpty(imageRequest.downloadHandler.text))
                    {
                        Debug.LogError($"Response Body: {imageRequest.downloadHandler.text}");
                    }
                }
            }
        }
    }

    public void LoadNextMovie()
    {
        currentMovieIndex++;
        StartCoroutine(LoadCurrentMovie());
    }
}

[System.Serializable]
public class MovieListResponse
{
    public List<MovieData> results;
}
