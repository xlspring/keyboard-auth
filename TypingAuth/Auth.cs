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
        if (_stopwatchPhrase.IsRunning && _UserText[^1] == ' ')
        {
            _stopwatchWord.Stop();
            TypingSpeedByWord.Add(_stopwatchWord.ElapsedMilliseconds);
            AvgTypingSpeedByWord = TypingSpeedByWord.Sum() / TypingSpeedByWord.Count;
            _stopwatchWord.Restart();
        }

        if (!Phrase.Contains(_UserText))
        {
            TypingErrors++;
        }
    }

    public void Start()
    {
        _stopwatchPhrase.Start();
        _stopwatchWord.Start();
    }
    // public User Compare()
    // {
    //     Console.WriteLine($"AvgTypingSpeedByWord: {AvgTypingSpeedByWord}, TypingErrors: {TypingErrors}");
    //
    //     _stopwatchPhrase.Stop();
    //     _stopwatchWord.Stop();
    //     TypingSpeed = _stopwatchPhrase.ElapsedMilliseconds;
    //     
    //     List<User> similarity = new List<User>();
    //     
    //     foreach (var user in Users)
    //     {
    //         Console.WriteLine($"{user.Name}, {user.Errors}, {user.AvgWordTypingSpeed}");
    //         
    //         double normalizedUserAvgSpeed = user.AvgWordTypingSpeed / 100.0;
    //         double normalizedResultAvgSpeed = AvgTypingSpeedByWord / 100.0;
    //         
    //         double userLength = Math.Sqrt(Math.Pow(normalizedUserAvgSpeed, 2) + Math.Pow(user.Errors, 2));
    //         double resultsLength = Math.Sqrt(Math.Pow(normalizedResultAvgSpeed, 2) + Math.Pow(TypingErrors, 2));
    //
    //         if (userLength == 0 || resultsLength == 0)
    //         {
    //             Console.WriteLine($"{user.Name}: Skipped due to zero vector length");
    //             continue; // Skip this user if division by zero would happen
    //         }
    //
    //         double distance = (user.AvgWordTypingSpeed * AvgTypingSpeedByWord) / (userLength * resultsLength);
    //
    //         Console.WriteLine($"{user.Name}: {distance}");
    //
    //         if (distance >= 0.8)
    //         {
    //             similarity.Add(user);
    //         }
    //     }
    //     
    //     return similarity.FirstOrDefault();
    // }
    
    public User? Compare()
    {
        _stopwatchPhrase.Stop();

        TypingSpeed = _stopwatchWord.ElapsedMilliseconds;

        Console.WriteLine($"AvgTypingSpeedByWord: {AvgTypingSpeedByWord}, TypingErrors: {TypingErrors}");

        List<User> similarity = new List<User>();

        foreach (var user in Users)
        {
            Console.WriteLine($"{user.Name}, {user.Errors}, {user.AvgWordTypingSpeed}");

            // Normalize errors to be comparable to typing speed
            double normalizedUserErrors = user.Errors / 100.0;
            double normalizedTypingErrors = TypingErrors / 100.0;

            double userLength = Math.Sqrt(Math.Pow(user.AvgWordTypingSpeed, 2) + Math.Pow(normalizedUserErrors, 2));
            double resultsLength = Math.Sqrt(Math.Pow(AvgTypingSpeedByWord, 2) + Math.Pow(normalizedTypingErrors, 2));

            if (userLength == 0 || resultsLength == 0)
            {
                Console.WriteLine($"{user.Name}: Skipped due to zero vector length");
                continue;
            }

            double distance = (user.AvgWordTypingSpeed * AvgTypingSpeedByWord + normalizedUserErrors * normalizedTypingErrors)
                              / (userLength * resultsLength);

            Console.WriteLine($"{user.Name}: {distance}");

            if (distance >= 0.8)
            {
                similarity.Add(user);
            }
        }

        return similarity.FirstOrDefault();
    }

}