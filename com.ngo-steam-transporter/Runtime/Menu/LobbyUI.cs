using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class LobbyUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _panelMain;       // Host, Join, Exit butonları
        [SerializeField] private GameObject _panelJoinInput;  // Kod girme alanı
        [SerializeField] private GameObject _panelLobby;      // Bekleme odası

        [Header("Main Menu Buttons")]
        [SerializeField] private Button _btnHost;
        [SerializeField] private Button _btnJoinMenu; // Join paneli açar
        [SerializeField] private Button _btnExit;

        [Header("Join Input Panel")]
        [SerializeField] private TMP_InputField _inputJoinCode;
        [SerializeField] private Button _btnConnect;
        [SerializeField] private Button _btnBackToMain;

        [Header("Lobby Panel")]
        [SerializeField] private TextMeshProUGUI _txtLobbyCode;
        [SerializeField] private TextMeshProUGUI _txtPlayerCount;
        [SerializeField] private Button _btnStartGame;
        [SerializeField] private Button _btnLeaveLobby;

        private void Start()
        {
            // Main Menu
            _btnHost.onClick.AddListener(OnHostClicked);
            _btnJoinMenu.onClick.AddListener(() => SwitchPanel(_panelJoinInput));
            _btnExit.onClick.AddListener(Application.Quit);

            // Join Input
            _btnConnect.onClick.AddListener(OnConnectClicked);
            _btnBackToMain.onClick.AddListener(() => SwitchPanel(_panelMain));

            // Lobby
            _btnStartGame.onClick.AddListener(OnStartGameClicked);
            _btnLeaveLobby.onClick.AddListener(OnLeaveClicked);

            SwitchPanel(_panelMain);
        }

        private void Update()
        {
            // Lobby panelindeysek oyuncu sayısını güncelle
            if (_panelLobby.activeSelf && LobbyManager.Instance)
            {
                _txtPlayerCount.text = $"Players Connected: {LobbyManager.Instance.GetPlayerCount()}";
                
                // Start butonu sadece Host için aktif
                if (NetworkManager.Singleton)
                {
                    _btnStartGame.interactable = NetworkManager.Singleton.IsHost;
                    _btnStartGame.gameObject.SetActive(NetworkManager.Singleton.IsHost); // Client görmesin
                }
            }
        }

        // --- ACTIONS ---

        private void OnHostClicked()
        {
            _btnHost.interactable = false; // Çift tıklamayı önle
            LobbyManager.Instance.HostGame((code) =>
            {
                if (code != null)
                {
                    _txtLobbyCode.text = code; // Kodu ekrana yaz
                    SwitchPanel(_panelLobby);
                }
                else
                {
                    Debug.LogError("Host başlatılamadı!");
                }
                _btnHost.interactable = true;
            });
        }

        private void OnConnectClicked()
        {
            string code = _inputJoinCode.text.Trim();
            if (string.IsNullOrEmpty(code)) return;

            _btnConnect.interactable = false;
            LobbyManager.Instance.JoinGame(code, (success) =>
            {
                if (success)
                {
                    _txtLobbyCode.text = $"Lobby: {code}";
                    SwitchPanel(_panelLobby);
                }
                else
                {
                    Debug.LogError("Bağlantı başarısız!");
                    _inputJoinCode.text = ""; // Kodu temizle
                }
                _btnConnect.interactable = true;
            });
        }

        private void OnStartGameClicked()
        {
            LobbyManager.Instance.StartGame();
        }

        private void OnLeaveClicked()
        {
            NetworkManager.Singleton.Shutdown();
            SwitchPanel(_panelMain);
        }

        private void SwitchPanel(GameObject targetPanel)
        {
            _panelMain.SetActive(false);
            _panelJoinInput.SetActive(false);
            _panelLobby.SetActive(false);

            targetPanel.SetActive(true);
        }
    }
}