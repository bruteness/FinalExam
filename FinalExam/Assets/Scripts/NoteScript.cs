using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Note", menuName = "ScriptableObjects/Note")]
public class NoteScript : ScriptableObject {
    [TextArea(10, 40)]
    [SerializeField] private string _noteText;

    public string NoteText { get { return _noteText; } }
}
