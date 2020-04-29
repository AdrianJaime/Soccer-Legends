using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerSelector_Representation : MonoBehaviourPun, IPunObservable
{

    public List<CharacterBasic> listActualCharacters = new List<CharacterBasic>();

    /// <summary> Encapsular esto tambein para el inventario script comun
    [SerializeField]
    CharactersCompendium inventory;

    public string[] arrayID = new string[8];//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    /// <summary>
    /// Esta clase tiene que ser de tipo CharacterBasicDB
    /// </summary>
    public CharacterBasic[] arrayCharactersBase ;//DESCARGAMOS UNA LISTA DE INTS con la info del ID de cada personaje equipado en el team desde BD A traves del identifierEquip

    // Start is called before the first frame update
    void Awake()
    {
        sendTeam();
    }

    public void sendTeam()
    {
        for (int i = 0; i < 8; i++)
        {
            arrayID[i] = PlayerPrefs.GetString("team" + "1" + "slot" + i);
            Debug.Log("In " + "team" + "1" + "slot" + i + " There is: " + PlayerPrefs.GetString("team" + "1" + "slot" + i));
        }
        photonView.RPC("setTeam", RpcTarget.OthersBuffered, arrayID);
    }

    [PunRPC]
    public void setTeam(string[] _arrayID)
    {
        transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = 
            PhotonNetwork.PlayerListOthers[0].NickName;
        int aux = 0;
        foreach (string value in _arrayID)
        {
            if (value != null)
            {
                Debug.Log("BUSCO UN ID EN EL INVENTARIO");
                CharacterInfo auxCharacter = inventory.compendiumOfCharacters.Find(x => x.ID == value);
                if (auxCharacter != null)
                {
                    Debug.Log("LO ENCONTRÉ");
                    transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(aux)
                        .GetComponent<CharacterBasic>().basicInfo = auxCharacter;
                    transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetChild(aux)
                        .GetComponent<PlayerSelector_CardRender>().UpdateRender();
                }
                else
                    Debug.Log("NO LO ENCONTRÉ DEJA DE PIRATEAR Y PONER IDs ERRONEOS");

            }
            aux++;

        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) // Send position to the other player. Stream == Getter.
    { }

    }
