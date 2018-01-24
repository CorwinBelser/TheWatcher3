using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

    //keeps tracks of sentences being passed through the conversation
    private Queue<string> sentences;
    public Text dialogueText;
    public Text nameText;
    public Animator animator;

	// Use this for initialization
	void Start () {
        sentences = new Queue<string>();
        animator.SetBool("isOpen", false);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Wowee, the dialogue for " + dialogue.name + " is playing.");
        nameText.text = dialogue.name;

        animator.SetBool("isOpen", true);

        sentences.Clear();
        
        foreach(string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndConversation();
        }

        string sentence = sentences.Dequeue();
        Debug.Log(sentence);
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndConversation()
    {
        animator.SetBool("isOpen", false);
        Debug.Log("That's all folks");
        return;
    }
}
