using UnityEngine;


[CreateAssetMenu(fileName = "NewNpcDialog", menuName = "NPC Dialog")]
public class NpcDialog : ScriptableObject
{
    public string npcName;
    public Sprite npcPortrait;
    public string[] dialogLines;
    public float typingSpeed = 0.05f;
    public AudioClip voiceSound;
    public float voicePitch = 1f;
    public bool[] autoProgressLines;
    public float autoProgressDelay = 1.5f;
    
}
