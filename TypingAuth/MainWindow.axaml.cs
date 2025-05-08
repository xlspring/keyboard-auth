using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
namespace TypingAuth;

public partial class MainWindow : Window
{
    Auth _auth = new Auth();
    private Popup authResultPopup;
    private TextBlock authResultText;
    
    public MainWindow()
    {
        InitializeComponent();
        
        // Get references to UI elements
        authResultPopup = this.FindControl<Popup>("AuthResultPopup");
        authResultText = this.FindControl<TextBlock>("AuthResultText");
        
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
        Console.WriteLine("Authentication attempt initiated");
        try 
        {
            User user = _auth.Compare();
            
            // Display authentication result in popup
            authResultText.Text = $"Authentication successful!\nWelcome, {user.Name}.\n\nYour typing pattern matched with a similarity score.";
            authResultPopup.IsOpen = true;
        }
        catch (Exception ex)
        {
            // Display authentication failure in popup
            authResultText.Text = $"Authentication failed!\n\n{ex.Message}";
            authResultPopup.IsOpen = true;
        }
    }
    
    private void ClosePopup_Click(object sender, RoutedEventArgs e)
    {
        // Close the authentication result popup
        authResultPopup.IsOpen = false;
    }
}