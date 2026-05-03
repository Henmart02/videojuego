using UnityEngine;
using System.Collections.Generic;

public class BughouseManager : MonoBehaviour
{
    public static BughouseManager Instance;

    [Header("Boards")]
    public BoardManager boardA;
    public BoardManager boardB;

    // Inventarios: piezas disponibles para colocar
    private Dictionary<string, List<PieceType>> pockets = new Dictionary<string, List<PieceType>>();

    private void Awake()
    {
        Instance = this;

        // Crear bolsillos vacíos
        pockets["BoardA_White"] = new List<PieceType>();
        pockets["BoardA_Black"] = new List<PieceType>();
        pockets["BoardB_White"] = new List<PieceType>();
        pockets["BoardB_Black"] = new List<PieceType>();
    }

    public BoardManager GetPartnerBoard(BoardManager currentBoard)
    {
        if (currentBoard == boardA) return boardB;
        if (currentBoard == boardB) return boardA;
        return null;
    }

    public int GetTeam(BoardManager board, bool isWhite)
    {
        if (board == boardA)
            return isWhite ? 1 : 2;

        if (board == boardB)
            return isWhite ? 2 : 1;

        return -1;
    }

    public bool GetPartnerColor(BoardManager currentBoard, bool isWhite)
    {
        int team = GetTeam(currentBoard, isWhite);
        BoardManager partnerBoard = GetPartnerBoard(currentBoard);

        if (partnerBoard == null) return false;

        if (partnerBoard == boardA)
            return team == 1; // Team 1 en BoardA = blancas

        if (partnerBoard == boardB)
            return team == 2; // Team 2 en BoardB = blancas

        return false;
    }

    public string GetPartnerInfo(BoardManager currentBoard, bool isWhite)
    {
        BoardManager partnerBoard = GetPartnerBoard(currentBoard);
        bool partnerColor = GetPartnerColor(currentBoard, isWhite);

        if (partnerBoard == null)
            return "Sin compañero";

        return "Compañero en " + partnerBoard.name + " con " + (partnerColor ? "blancas" : "negras");
    }

    private string GetPocketKey(BoardManager board, bool isWhite)
    {
        string boardName = board == boardA ? "BoardA" : "BoardB";
        string colorName = isWhite ? "White" : "Black";
        return boardName + "_" + colorName;
    }

    public void AddCapturedPieceForPartner(BoardManager sourceBoard, bool capturerIsWhite, PieceType capturedPieceType)
    {
        BoardManager partnerBoard = GetPartnerBoard(sourceBoard);
        bool partnerColor = GetPartnerColor(sourceBoard, capturerIsWhite);

        if (partnerBoard == null)
        {
            Debug.LogWarning("No se encontró tablero compañero.");
            return;
        }

        string key = GetPocketKey(partnerBoard, partnerColor);
        pockets[key].Add(capturedPieceType);

        Debug.Log("Se agregó " + capturedPieceType + " al inventario de "
            + partnerBoard.name + " "
            + (partnerColor ? "blancas" : "negras"));

        DebugPocket(key);

        if (GameManager.Instance != null)
    GameManager.Instance.RefreshAllPocketButtons();
    }

    public List<PieceType> GetPocket(BoardManager board, bool isWhite)
    {
        string key = GetPocketKey(board, isWhite);
        return pockets[key];
    }

    public bool RemovePieceFromPocket(BoardManager board, bool isWhite, PieceType pieceType)
    {
        string key = GetPocketKey(board, isWhite);

        if (pockets[key].Contains(pieceType))
        {
            pockets[key].Remove(pieceType);
            Debug.Log("Se removió " + pieceType + " del inventario de " + key);
            DebugPocket(key);
            return true;
        }

        return false;
    }

    private void DebugPocket(string key)
    {
        string content = key + ": ";

        if (pockets[key].Count == 0)
        {
            content += "vacío";
        }
        else
        {
            for (int i = 0; i < pockets[key].Count; i++)
            {
                content += pockets[key][i];
                if (i < pockets[key].Count - 1)
                    content += ", ";
            }
        }

        Debug.Log(content);
    }
}