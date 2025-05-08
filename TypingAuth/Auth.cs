using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TypingAuth;

public class Auth()
{
    public string? Phrase { get; set; }

    public string UserText
    {
        get => _UserText;
        set
        {
            _UserText = value;
            _UpdateCalculations();
        }
    }

    private string _UserText;

    public long TypingSpeed { get; set; }
    public List<long> TypingSpeedByWord = new List<long>();
    public int TypingErrors = 0;
    public long AvgTypingSpeedByWord = 0;

    private Stopwatch _stopwatchPhrase = new Stopwatch();
    private Stopwatch _stopwatchWord = new Stopwatch();

    private List<User> Users = new List<User>()
    {
        new User("pfxel", 20, 2300),
        new User("testUser", 20, 4000),
        new User("fastUser", 5, 1000)
    };

    private void _UpdateCalculations()
    {
        if (_stopwatchPhrase.IsRunning && !string.IsNullOrEmpty(_UserText) && _UserText.Length > 0 && _UserText[^1] == ' ')
        {
            _stopwatchWord.Stop();
            TypingSpeedByWord.Add(_stopwatchWord.ElapsedMilliseconds);
            AvgTypingSpeedByWord = TypingSpeedByWord.Sum() / TypingSpeedByWord.Count;
            _stopwatchWord.Restart();
        }

        if (Phrase != null && _UserText != null)
        {
            // Reset error count before checking
            TypingErrors = 0;
            
            // Track positions where errors have been found
            HashSet<int> errorPositions = new HashSet<int>();
            
            // Compare each character of UserText with corresponding character in Phrase
            for (int i = 0; i < _UserText.Length; i++)
            {
                // If we've gone beyond the phrase length or characters don't match
                if (i >= Phrase.Length || _UserText[i] != Phrase[i])
                {
                    // Only count as a new error if we haven't seen this position before
                    if (!errorPositions.Contains(i))
                    {
                        TypingErrors++;
                        errorPositions.Add(i);
                    }
                }
            }
        }
    }

    public void Start()
    {
        _stopwatchPhrase.Start();
        _stopwatchWord.Start();
    }
    
    public User Compare()
    {
        Console.WriteLine($"AvgTypingSpeedByWord: {AvgTypingSpeedByWord}, TypingErrors: {TypingErrors}");
    
        _stopwatchPhrase.Stop();
        _stopwatchWord.Stop();
        
        TypingSpeed = _stopwatchPhrase.ElapsedMilliseconds;
        
        User mostSimilarUser = null;
        double highestSimilarity = 0;
        
        long minSpeed = Users.Min(u => u.AvgWordTypingSpeed);
        long maxSpeed = Users.Max(u => u.AvgWordTypingSpeed);
        int minErrors = Users.Min(u => u.Errors);
        int maxErrors = Users.Max(u => u.Errors);
        
        long speedRange = maxSpeed - minSpeed;
        int errorRange = maxErrors - minErrors;
        
        if (speedRange == 0) speedRange = 1;
        if (errorRange == 0) errorRange = 1;
        
        double normalizedCurrentSpeed = (double)(AvgTypingSpeedByWord - minSpeed) / speedRange;
        double normalizedCurrentErrors = (double)(TypingErrors - minErrors) / errorRange;
        
        foreach (var user in Users)
        {
            Console.WriteLine($"Comparing with: {user.Name}, Errors: {user.Errors}, AvgSpeed: {user.AvgWordTypingSpeed}");
            
            double normalizedUserSpeed = (double)(user.AvgWordTypingSpeed - minSpeed) / speedRange;
            double normalizedUserErrors = (double)(user.Errors - minErrors) / errorRange;
            
            double distance = Math.Sqrt(
                Math.Pow(normalizedUserSpeed - normalizedCurrentSpeed, 2) + 
                Math.Pow(normalizedUserErrors - normalizedCurrentErrors, 2)
            );
            
            double similarity = 1 / (1 + distance);
            
            Console.WriteLine($"{user.Name}: Similarity score = {similarity:F4}");
            
            if (similarity > highestSimilarity)
            {
                highestSimilarity = similarity;
                mostSimilarUser = user;
            }
        }

        if (mostSimilarUser == null)
        {
            throw new NullReferenceException("No similar user found");
        }
        
        Console.WriteLine($"Most similar user: {mostSimilarUser.Name} with score {highestSimilarity:F4}");
        return mostSimilarUser;
    }
}