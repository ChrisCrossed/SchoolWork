using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;

/*******************************************************************************
filename    Cs_PullRiotAPI.cpp
author      Chris Christensen
email       ChrisCrossed.86@Gmail.com

Brief Description:
  This program grabs various Riot API information when necessary.
  
*******************************************************************************/

enum RegionLocations
{
    NA, EUW, BR, EUNE, KR, LAN, LAS, OCE, RU, TR
}

public class SummonerInfo
{
    public int summonerID;
    public string name;
    public int profileIconId;
    public int summonerLevel;
    public int revisionDate;
}

public class ShoutcasterInfo
{
    // Match information (Holds the info to files)
    public int i_MatchID;
    public string s_SaveAPIInfo;

    // Caster Information
    public string s_CasterAPIKey;
    public string s_CasterUsername;
    public int summonerID;
}

public class PlayerData
{
    public ShoutcasterInfo shoutcaster = new ShoutcasterInfo();

    // Red Team Information
    public SummonerInfo player_Red_One   = new SummonerInfo();
    public SummonerInfo player_Red_Two   = new SummonerInfo();
    public SummonerInfo player_Red_Three = new SummonerInfo();
    public SummonerInfo player_Red_Four  = new SummonerInfo();
    public SummonerInfo player_Red_Five  = new SummonerInfo();

    // Blue Team Information
    public SummonerInfo player_Blue_One   = new SummonerInfo();
    public SummonerInfo player_Blue_Two   = new SummonerInfo();
    public SummonerInfo player_Blue_Three = new SummonerInfo();
    public SummonerInfo player_Blue_Four  = new SummonerInfo();
    public SummonerInfo player_Blue_Five  = new SummonerInfo();
}

public class Cs_PullRiotAPI : MonoBehaviour
{
    WWW www_ApiRequest;
    bool b_IsDone = false;

    RegionLocations enum_RegionLocation;

    PlayerData playerData = new PlayerData();

    // Use this for initialization
    void Start()
    {
        // Forcing in my own key to test. MUST replace later.
        playerData.shoutcaster.s_CasterAPIKey = "79ba48bc-e49a-4a64-a3ee-55ac3d012c24";
        playerData.shoutcaster.s_CasterUsername = "ChrisCrossed";
        enum_RegionLocation = RegionLocations.NA;

        // Before each match, find my summonerID based off my username.
        // int i_SummID_By_Username = GetSummonerIDByName(s_CasterUsername);
        DownloadAPI_Summoner_1_4(RegionLocations.NA, "ChrisCrossed");
    }

    /*******************************************************************************

        Function:   DownloadAPI_Summoner_1_4
     Description:   Downloads the Summoner 1.4 API. For use when finding SummonerID

          Inputs:   enum_RegionLocation_ (RegionLocation) - World Location
                    s_SummonerName_ (String) - The Username being searched for

         Outputs:   i_SummonerID (Int) - The number Riot provides for their username

    *******************************************************************************/
    void DownloadAPI_Summoner_1_4(RegionLocations enum_RegionLocation_, string s_SummonerName_)
    {
        // Reference Link:
        // https://na.api.pvp.net/api/lol/na/v1.4/summoner/by-name/ChrisCrossed?api_key=79ba48bc-e49a-4a64-a3ee-55ac3d012c24

        // Create the proper website link
        string s_LinkURL = "https://" +
                            enum_RegionLocation_.ToString() +
                            ".api.pvp.net/api/lol/" +
                            enum_RegionLocation_.ToString() +
                            "/v1.4/summoner/by-name/" +
                            s_SummonerName_.ToString() + 
                            "?api_key=" + playerData.shoutcaster.s_CasterAPIKey;

        APIRequest(s_LinkURL, true);
    }

    /*******************************************************************************

        Function:   APIRequest
     Description:   Sends out the request to the internet for information.
                    Is run by Update() every frame to check when a request completes.

          Inputs:   s_WebAPILink_ (String) - The website request in string form
                    b_IsNewRequest_ (Bool) - Resets the Request
                    OutputFunction_ (Function) - The function to call when complete

         Outputs:   None

    *******************************************************************************/
    void APIRequest(string s_WebAPILink_, bool b_IsNewRequest_)
    {
        if(b_IsNewRequest_)
        {
            // This sends out the API Info online. www_ApiRequest.IsDone states if complete.
            www_ApiRequest = new WWW(s_WebAPILink_);

            // States that a request has been made. Turns off later so I don't keep requesting
            b_IsDone = true;
        }
        else
        {
            if (www_ApiRequest.isDone && b_IsDone)
            {
                playerData.shoutcaster.s_SaveAPIInfo = www_ApiRequest.text;
                
                // Sets the Caster Information (Nasty parsing ahead... had no choice)
                if(playerData.shoutcaster.s_SaveAPIInfo.Contains("\"id\":"))
                {
                    // Start with a fresh string
                    string testOutput;

                    // Find the location of 'id' within the API string
                    int startPos = playerData.shoutcaster.s_SaveAPIInfo.IndexOf("\"id\":") + 5;
                    testOutput = playerData.shoutcaster.s_SaveAPIInfo.Substring(startPos);

                    // Find the end location of the first piece of information
                    startPos = testOutput.IndexOf(',');
                    testOutput = testOutput.Substring(0, startPos);

                    // Passes the information along
                    playerData.shoutcaster.summonerID = int.Parse(testOutput);
                    print(playerData.shoutcaster.summonerID);

                    // **********************************************************************

                    // Find the location of 'Name' within the API string
                    print(playerData.shoutcaster.s_SaveAPIInfo);
                    startPos = playerData.shoutcaster.s_SaveAPIInfo.IndexOf("\"name\":") + 8;
                    testOutput = playerData.shoutcaster.s_SaveAPIInfo.Substring(startPos);

                    // Cut off the remaining quotation marks
                    startPos = testOutput.IndexOf("\",");
                    testOutput = testOutput.Substring(0, startPos);

                    // Store the name of the shoutcaster
                    playerData.shoutcaster.s_CasterUsername = testOutput;
                    print(playerData.shoutcaster.s_CasterUsername);
                }
                // {"id":35703666,"name":"ChrisCrossed","profileIconId":749,"summonerLevel":30,"revisionDate":1453107512000}

                b_IsDone = false;
            }
        }
    }

    // Update is called once per frame
    void Update ()
    {
        APIRequest(null, false);
    }
}
