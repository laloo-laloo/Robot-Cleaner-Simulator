using UnityEngine;

[System.Serializable]
public class CutscenePage
{
    public enum FlipDirection { LeftToRight, RightToLeft }

    [Header("Page Content")]
    public Sprite illustrationSprite;       // 크레파스 인트로 일러스트 (4장)
    [TextArea(3, 5)]
    public string storyText;                // 동화책 대사/설명 문구

    [Header("Page Flip Settings")]
    public FlipDirection flipDirection = FlipDirection.RightToLeft; // 책장 넘기는 방향
}
