using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    // text box input field reference
    [SerializeField] private TMP_InputField name_input_field_ = null;

    // continue button reference
    [SerializeField] private Button continue_button_ = null;

    // static string variable that can easily be referenced from elsewhere 
    // while still having scoped editting rights
    public static string display_name_ { get; private set; }

    // unique constant key
    private const string player_prefs_name_key_ = "PlayerName";

    private void Start() => SetUpInputField();

    private void SetUpInputField()
    {
        // return if there is no default name value stored in player prefs
        if (!PlayerPrefs.HasKey(player_prefs_name_key_)) { return; }

        // set the default name to Player prefs "PlayerName" key
        string default_name = PlayerPrefs.GetString(player_prefs_name_key_);

        // fill the input field with the deafault player name
        name_input_field_.text = default_name;

        SetPlayerName(default_name);
    }

    public void SetPlayerName(string name)
    {
        // only allow continue button to be pressed if name is not empty
        continue_button_.interactable = !string.IsNullOrEmpty(name);
    }

    public void SavePlayerName()
        // a function to be called once the continue button is pressed
    {
        // set the players name to the inputted text
        display_name_ = name_input_field_.text;

        // set the default player name in player prefs for future.
        PlayerPrefs.SetString(player_prefs_name_key_, display_name_);
    }
}
