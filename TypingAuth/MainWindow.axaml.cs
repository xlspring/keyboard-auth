using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace TypingAuth;

public partial class MainWindow : Window
{
    Auth _auth = new Auth();
    public MainWindow()
    {
        InitializeComponent();
        
        string jsonString = File.ReadAllText("phrases.json");
        Phrase phrase = JsonSerializer.Deserialize<Phrase>(jsonString)!;
        
        Random random = new Random();
        int phraseNumber = random.Next(0, 7);
        
        _auth.Phrase = phrase.Phrases[phraseNumber];
        
        PhraseDisplay.Text = _auth.Phrase;
        _auth.Start();
    }

    private void PhraseTextBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            string newText = textBox.Text ?? string.Empty;
            
            _auth.UserText = newText;
            
            ErrorDisplay.Text = _auth.TypingErrors.ToString();
            AvgWordDisplay.Text = _auth.AvgTypingSpeedByWord.ToString();
        }
    }

    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Pointer pressed");
        User user = _auth.Compare();
    }
}