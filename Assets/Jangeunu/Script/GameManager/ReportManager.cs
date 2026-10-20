using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ReportManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField _inputField; // 텍스트 입력창
    [SerializeField] private Button _sendButton;         // 전송 버튼
    [SerializeField] private GameObject _reportPanel;    // 버그 문의 패널 자체

    // 1단계에서 복사해 둔 구글 웹 앱 URL을 여기에 붙여넣습니다.
    private string _googleScriptUrl = "https://script.google.com/macros/s/AKfycbyq0b_faiHiixOKtSbKZyJLELT6E7TuPkdpGYrb5_Q5U0CSvEDf1Ms2nx84M4DJe1MnsQ/exec";

    private void Start()
    {
        if (_sendButton != null)
        {
            _sendButton.onClick.AddListener(OnClickSendReport);
        }
    }

    private void OnClickSendReport()
    {
        string reportText = _inputField.text;

        if (string.IsNullOrWhiteSpace(reportText))
        {
            Debug.Log("내용을 입력해주세요!");
            return;
        }

        StartCoroutine(SendToGoogleSheet(reportText));
    }

    private IEnumerator SendToGoogleSheet(string content)
    {
        // 보낼 데이터를 JSON 형태로 포장
        string jsonPayload = "{\"content\":\"" + content.Replace("\"", "\\\"") + "\"}";
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);

        UnityWebRequest www = new UnityWebRequest(_googleScriptUrl, "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("건의사항 전송 성공!");

            // 입력창 비우고 패널 닫기
            _inputField.text = "";
            if (_reportPanel != null)
                _reportPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("전송 실패: " + www.error);
        }
    }
}
