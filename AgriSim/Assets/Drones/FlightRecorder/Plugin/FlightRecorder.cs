using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

namespace FlightRecorderPlugin
{

	#region Classes and Structures

	[System.Serializable]
	public class DataReadingStructure
	{
		public Vector3 Position;
		public Vector3 RotationFixed;
		public Vector3 Rotation;
		public Vector3 RotationAnimated;
		public float Vertical_W = 0;
		public float Vertical_S = 0;
		public float Horizontal_A = 0;
		public float Horizontal_D = 0;
		public float Vertical_I = 0;
		public float Vertical_K = 0;
		public float Horizontal_J = 0;
		public float Horizontal_L = 0;
		public Vector3 Velocity;

		public DataReadingStructure(
			Vector3 position,
			Vector3 fixedRotation,
			Vector3 rotation,
			Vector3 rotationAnimated,
			float vertical_W,
			float vertical_S,
			float horizontal_A,
			float horizontal_D,
			float vertical_I,
			float vertical_K,
			float horizontal_J,
			float horizontal_L,
			Vector3 velocity)
		{
			Position = position;
			RotationFixed = fixedRotation;
			Rotation = rotation;
			RotationAnimated = rotationAnimated;
			Vertical_W = vertical_W;
			Vertical_S = vertical_S;
			Horizontal_A = horizontal_A;
			Horizontal_D = horizontal_D;
			Vertical_I = vertical_I;
			Vertical_K = vertical_K;
			Horizontal_J = horizontal_J;
			Horizontal_L = horizontal_L;
			Velocity = velocity;
		}
	}

	[System.Serializable]
	public struct LoadFlightStructure
	{
		//public GameObject _buttonPrefab;
		//public Transform _content;

		//Save file fields,rects,buttons
		public RectTransform _saveFileContent;
		public InputField _fileNameInput;
		public Button _saveFileNameButton;
	}

	#endregion

	public class FlightRecorder : MonoBehaviour
	{

		public static FlightRecorder Instance;
		public int FPS;
		public bool Recording
		{
			get
			{
				return _recording;
			}
		}
		public string FolderPath
		{
			set
			{
				_folderPath = value;
			}
			get
			{
				return _folderPath;
			}
		}

		[SerializeField] private bool _recording;
		[SerializeField, TextArea(3, 15)] private string _folderPath;
		[SerializeField] private LoadFlightStructure _loadFlightStructure;

		[Space(10)]
		public FlightRecorderPlayback PlaybackScript;

		public delegate void FlightRecorderEvents();
		public event FlightRecorderEvents OnRecordingStart;
		public event FlightRecorderEvents OnRecordingStop;

		#region MonoBehaviour Methods

		private void Awake()
		{
			if(Instance == null)
			{
				Instance = this;
			}
		}

		private void OnDisable()
		{
			StopRecording();
		}

		private void Update()
		{
			FPS = (int)(1.0f / Time.deltaTime);
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 60;
		}

		#endregion

		#region Storing and Reading data from and to our .txt file

		/// <summary>
		/// Data that we store every frame, that is parsed to string and stored per line in our .txt file.
		/// This data gets passed to the method that saves our data into the actual .txt file.
		/// </summary>
		List<string> inputData;
		/// <summary>
		/// Recording happens here... depending on the FPS... I hope you have an okay bucket (PC) ;)
		/// </summary>
		/// <returns></returns>
		IEnumerator Coroutine_Recording()
		{
			inputData = new List<string>();
			while (_recording)
			{
				yield return null;
				string decimalPoint = "F5"; //how precise input do you want to capture
											//roll,pitch,yaw,throttle in that particular order as in betaflight
				string posX = PlaybackScript.GetComponent<Rigidbody>().position.x.ToString();
				string posY = PlaybackScript.GetComponent<Rigidbody>().position.y.ToString();
				string posZ = PlaybackScript.GetComponent<Rigidbody>().position.z.ToString();
				string posXFixed = PlaybackScript.droneMovement.droneObject.localRotation.eulerAngles.x.ToString();
				string posYFixed = PlaybackScript.droneMovement.droneObject.localRotation.eulerAngles.y.ToString();
				string posZFixed = PlaybackScript.droneMovement.droneObject.localRotation.eulerAngles.z.ToString();
				string rotX = PlaybackScript.GetComponent<Rigidbody>().rotation.eulerAngles.x.ToString();
				string rotY = PlaybackScript.GetComponent<Rigidbody>().rotation.eulerAngles.y.ToString();
				string rotZ = PlaybackScript.GetComponent<Rigidbody>().rotation.eulerAngles.z.ToString();
				string rotAnimatedX = PlaybackScript.droneMovement.animatedGameObject.transform.localRotation.eulerAngles.x.ToString();
				string rotAnimatedY = PlaybackScript.droneMovement.animatedGameObject.transform.localRotation.eulerAngles.y.ToString();
				string rotAnimatedZ = PlaybackScript.droneMovement.animatedGameObject.transform.localRotation.eulerAngles.z.ToString();
				string velocityX = PlaybackScript.GetComponent<Rigidbody>().velocity.x.ToString();
				string velocityY = PlaybackScript.GetComponent<Rigidbody>().velocity.y.ToString();
				string velocityZ = PlaybackScript.GetComponent<Rigidbody>().velocity.z.ToString();
				string _dataToAdd =
					"(" + posX + "," + posY + "," + posZ + ")" + "#" +
					"(" + posXFixed + "," + posYFixed + "," + posZFixed + ")" + "#" +
					"(" + rotX + "," + rotY + "," + rotZ + ")" + "#" +
					"(" + rotAnimatedX + "," + rotAnimatedY + "," + rotAnimatedZ + ")" + "#" +
					PlaybackScript.Vertical_W.ToString() + "#" +
					PlaybackScript.Vertical_S.ToString() + "#" +
					PlaybackScript.Horizontal_A.ToString() + "#" +
					PlaybackScript.Horizontal_D.ToString() + "#" +
					PlaybackScript.Vertical_I.ToString() + "#" +
					PlaybackScript.Vertical_K.ToString() + "#" +
					PlaybackScript.Horizontal_J.ToString() + "#" +
					PlaybackScript.Horizontal_L.ToString() + "#" +
					"(" + velocityX + "," + velocityY + "," + velocityZ + ")";
				inputData.Add(_dataToAdd);
			}

			if (OnRecordingStop != null) OnRecordingStop();
		}

		/// <summary>
		/// Reading data from text file
		/// </summary>
		public DataReadingStructure[] ReadRecodedFile(string saveFilePath)
		{
			//if (!File.Exists(_loadedFlight))
			//{
			//	Debug.LogError("•The file you're trying to load does not exstis.");
			//	return null;
			//}

			if (!File.Exists(saveFilePath))
			{
				Debug.LogError("•The file you're trying to load does not exstis.", gameObject);
				return null;
			}

			List<DataReadingStructure> readData = new List<DataReadingStructure>();
			string line;
			//StreamReader stream = new StreamReader(_loadedFlight);
			StreamReader stream = new StreamReader(saveFilePath);
			while ((line = stream.ReadLine()) != null)
			{
				string[] incomingData = line.Split('#');

				string cleanPositionVector = incomingData[0];
				cleanPositionVector = cleanPositionVector.Replace("(", "");
				cleanPositionVector = cleanPositionVector.Replace(")", "");
				string[] vectorSplit = cleanPositionVector.Split(',');
				Vector3 position = new Vector3(
					float.Parse(vectorSplit[0]),
					float.Parse(vectorSplit[1]),
					float.Parse(vectorSplit[2])
					);

				string cleanRotationFixedVector = incomingData[1];
				cleanRotationFixedVector = cleanRotationFixedVector.Replace("(", "");
				cleanRotationFixedVector = cleanRotationFixedVector.Replace(")", "");
				string[] vectorRotationFixedSplit = cleanRotationFixedVector.Split(',');
				Vector3 rotationFixed = new Vector3(
					float.Parse(vectorRotationFixedSplit[0]),
					float.Parse(vectorRotationFixedSplit[1]),
					float.Parse(vectorRotationFixedSplit[2])
					);

				string cleanRotationVector = incomingData[2];
				cleanRotationVector = cleanRotationVector.Replace("(", "");
				cleanRotationVector = cleanRotationVector.Replace(")", "");
				string[] vectorRotationSplit = cleanRotationVector.Split(',');
				Vector3 rotation = new Vector3(
					float.Parse(vectorRotationSplit[0]),
					float.Parse(vectorRotationSplit[1]),
					float.Parse(vectorRotationSplit[2])
					);

				string cleanRotationAnimatedVector = incomingData[3];
				cleanRotationAnimatedVector = cleanRotationAnimatedVector.Replace("(", "");
				cleanRotationAnimatedVector = cleanRotationAnimatedVector.Replace(")", "");
				string[] vectorRotationAnimatedSplit = cleanRotationAnimatedVector.Split(',');
				Vector3 rotationAnimated = new Vector3(
					float.Parse(vectorRotationAnimatedSplit[0]),
					float.Parse(vectorRotationAnimatedSplit[1]),
					float.Parse(vectorRotationAnimatedSplit[2])
					);

				string velocityDrone = incomingData[3 + 9];
				velocityDrone = velocityDrone.Replace("(", "");
				velocityDrone = velocityDrone.Replace(")", "");
				string[] velocitySplit = velocityDrone.Split(',');
				Vector3 velocityV = new Vector3(
					float.Parse(velocitySplit[0]),
					float.Parse(velocitySplit[1]),
					float.Parse(velocitySplit[2])
					);


				readData.Add(
					new DataReadingStructure(
						position,
						rotationFixed,
						rotation,
						rotationAnimated,
						float.Parse(incomingData[3 + 1]),       //input keys
						float.Parse(incomingData[3 + 2]),       //input keys
						float.Parse(incomingData[3 + 3]),       //input keys
						float.Parse(incomingData[3 + 4]),       //input keys
						float.Parse(incomingData[3 + 5]),       //input keys
						float.Parse(incomingData[3 + 6]),       //input keys
						float.Parse(incomingData[3 + 7]),       //input keys
						float.Parse(incomingData[3 + 8]),       //input keys
						velocityV								//velocity
						)
					);
			}
			return readData.ToArray();
		}

		#endregion


		List<Coroutine> C_Playback = new List<Coroutine>();
		public void StartPlayingPlayback(DataReadingStructure[] dataFromSaveFile, FlightRecorderPlayback flightRecorderPlayback, Action callback)
		{
			C_Playback.Add(StartCoroutine(Coroutine_Playback(dataFromSaveFile, flightRecorderPlayback, callback)));
		}

		IEnumerator Coroutine_Playback(DataReadingStructure[] dataFromSaveFile, FlightRecorderPlayback flightRecorderPlayback, Action callback)
		{
			Debug.Log("•Started playing playback");

			//Position on starting od recording point
			flightRecorderPlayback.GetComponent<Rigidbody>().position = dataFromSaveFile[0].Position;
			flightRecorderPlayback.droneMovement.droneObject.localRotation = Quaternion.Euler(dataFromSaveFile[0].RotationFixed);
			flightRecorderPlayback.GetComponent<Rigidbody>().rotation = Quaternion.Euler(dataFromSaveFile[2].Rotation);
			flightRecorderPlayback.droneMovement.animatedGameObject.transform.localRotation = Quaternion.Euler(dataFromSaveFile[0].RotationAnimated);

			//start playing data on our drone
			int i = 0;
			while (i < dataFromSaveFile.Length - 1)
			{
				yield return null;
				flightRecorderPlayback.GetComponent<Rigidbody>().position = Vector3.Lerp(flightRecorderPlayback.GetComponent<Rigidbody>().position, dataFromSaveFile[i].Position, Time.deltaTime * 15);
				flightRecorderPlayback.droneMovement.droneObject.localRotation = Quaternion.Euler(dataFromSaveFile[i].RotationFixed);
				flightRecorderPlayback.GetComponent<Rigidbody>().rotation = Quaternion.Euler(dataFromSaveFile[i].Rotation);
				flightRecorderPlayback.droneMovement.animatedGameObject.transform.localRotation = Quaternion.Euler(dataFromSaveFile[i].RotationAnimated);
				flightRecorderPlayback.Vertical_W = dataFromSaveFile[i].Vertical_W;
				flightRecorderPlayback.Vertical_S = dataFromSaveFile[i].Vertical_S;
				flightRecorderPlayback.Horizontal_A = dataFromSaveFile[i].Horizontal_A;
				flightRecorderPlayback.Horizontal_D = dataFromSaveFile[i].Horizontal_D;
				flightRecorderPlayback.Vertical_I = dataFromSaveFile[i].Vertical_I;
				flightRecorderPlayback.Vertical_K = dataFromSaveFile[i].Vertical_K;
				flightRecorderPlayback.Horizontal_J = dataFromSaveFile[i].Horizontal_J;
				flightRecorderPlayback.Horizontal_L = dataFromSaveFile[i].Horizontal_L;
				flightRecorderPlayback.GetComponent<Rigidbody>().velocity = dataFromSaveFile[i].Velocity;
				i++;
			}

			Debug.Log("•Finished playing playback");
			callback.Invoke();
		}

		#region UI Button Methods

		/// <summary>
		/// Just a refference to our coroutine.
		/// </summary>
		Coroutine c_Recording;
		/// <summary>
		/// Starts recording our flight path.
		/// </summary>
		public void StartRecording()
		{
			//prevent someone just calling recording and glitching, we want to start recording only if we we were previously recording
			if (_recording) return;

			Debug.Log("•Started recording");
			if (OnRecordingStart != null) OnRecordingStart(); //delegate call for other actions to link on this
			_recording = true;
			if (c_Recording != null) StopCoroutine(c_Recording);
			c_Recording = StartCoroutine(Coroutine_Recording());

			//remove lsiteners from rename button
			_loadFlightStructure._saveFileNameButton.onClick.RemoveAllListeners();
			//hide change save file name if user clicks star again and didn't rename its file
			if (inputToggled) if (gameObject.activeInHierarchy) StartCoroutine(Coroutine_HideSaveFileInput());
		}

		/// <summary>
		/// Stops recording our flight path.
		/// </summary>
		public void StopRecording()
		{
			//prevent someone just calling stop recording and glitching, we want to stop recording only if we were previously recording
			if (!_recording) return;

			Debug.Log("•Stopped recording");
			_recording = false;
			WriteDataToTextFile(inputData.ToArray());
		}

		//public void LoadFlight()
		//{
		//	//if folder does not exsits, or we don't have refference of it, create it or get the refference
		//	if (!Directory.Exists(_folderPath))
		//	{
		//		CreateDirectoryForRecords();
		//	}

		//	//read files in our folder, returns full paths
		//	string[] recordsList = Directory.GetFiles(_folderPath);

		//	//filter results and get only names
		//	string[] recordsListFiltered = new string[recordsList.Length]; //stored only file name here
		//	for (int i = 0; i < recordsList.Length; i++)
		//	{
		//		string[] splittedString = recordsList[i].Split('\\');
		//		recordsListFiltered[i] = splittedString[splittedString.Length - 1];
		//	}

		//	//Draw buttons to list view
		//	for (int i = 0; i < recordsListFiltered.Length; i++)
		//	{
		//		int iteratorIndex = i; //if we assign 'i' in a lambda function it will pass the refference of 'i' and not a value, we have to make a copy of that and then send as parameter
		//		GameObject tmpGo = Instantiate(_loadFlightStructure._buttonPrefab, _loadFlightStructure._content);
		//		tmpGo.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = recordsListFiltered[i];
		//		UnityEngine.UI.Button button = tmpGo.GetComponent<UnityEngine.UI.Button>();
		//		button.onClick.AddListener(() =>
		//		{
		//			_loadedFlight = _folderPath + "\\" + recordsListFiltered[iteratorIndex];
		//			Debug.Log("•Loaded: " + _loadedFlight);
		//		});
		//	}
		//}

		#endregion

		#region Folder and Files manipulation Methods

		/// <summary>
		/// Creating folder in MyDocuments.
		/// </summary>
		void CreateDirectoryForRecords()
		{
			string myDocumentsLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			string flightRecorderFolderName = "FlightRecorder";
			string finalDirectoryString = myDocumentsLocation + "\\" + flightRecorderFolderName;
			_folderPath = finalDirectoryString;

			if (Directory.Exists(finalDirectoryString))
			{
				Debug.Log("•Folder '" + flightRecorderFolderName + "' <color=red>already exists</color> on location: '" + myDocumentsLocation + "'");
			}
			else
			{
				Directory.CreateDirectory(FolderPath);
				Debug.Log("•'" + flightRecorderFolderName + "' folder created <color=green>successfully</color> on location: '" + myDocumentsLocation + "'");
			}
		}

		/// <summary>
		/// Creating text file after recording stopped.
		/// </summary>
		/// <returns></returns>
		string CreateTextFile()
		{
			string textFileLocation = _folderPath + "\\" + GenerateFileName("");
			StreamWriter sw = File.CreateText(textFileLocation);
			sw.Flush();
			sw.Close();

			return textFileLocation;
		}

		/// <summary>
		/// Writing data to our newly created text file
		/// </summary>
		/// <param name="_data"></param>
		void WriteDataToTextFile(string[] _data)
		{
			CreateDirectoryForRecords();
			string createdTextFileLocaton = CreateTextFile();
			File.WriteAllLines(createdTextFileLocaton, _data);

			if(gameObject.activeInHierarchy) StartCoroutine(Coroutine_ShowSaveFileInput());
			_loadFlightStructure._saveFileNameButton.onClick.AddListener(() =>
			{
				if(_loadFlightStructure._fileNameInput.text.Length != 0)
				{
					string renamedFileFullPathAndName = _folderPath + "\\" + _loadFlightStructure._fileNameInput.text + ".txt";
					if (!File.Exists(renamedFileFullPathAndName))
					{
						File.Move(createdTextFileLocaton, renamedFileFullPathAndName);
					}
					else
					{
						renamedFileFullPathAndName = _folderPath + "\\" + GenerateFileName(_loadFlightStructure._fileNameInput.text) + ".txt";
						File.Move(createdTextFileLocaton, renamedFileFullPathAndName);
					}
					Debug.Log("•Saved flight renamed to: '" + _loadFlightStructure._fileNameInput.text + "'");
				}
				if (gameObject.activeInHierarchy) StartCoroutine(Coroutine_HideSaveFileInput());
			});

			Debug.Log("•Flight saved to: '" + createdTextFileLocaton + "'");
		}

		/// <summary>
		/// USed to generate custom name if current name exists or  user did not input any name for his new save file.
		/// </summary>
		/// <param name="customName"></param>
		/// <returns></returns>
		string GenerateFileName(string customName)
		{
			string dateNow = System.DateTime.Now.ToString("dd mm yyyy hh mm ss");
			string textFileName = "FlightRecord ";
			string fileExtension = ".txt";
			string textFileLocation;

			if(customName.Length == 0) textFileLocation = customName + textFileName + dateNow + fileExtension;
			else textFileLocation = customName + " " + textFileName + dateNow + fileExtension;

			return textFileLocation;
		}

		#endregion

		#region Animation transitions

		bool inputToggled = false;
		IEnumerator Coroutine_ShowSaveFileInput()
		{
			inputToggled = true;
			float timer = 0;
			Vector2 currentPosition = _loadFlightStructure._saveFileContent.anchoredPosition;
			Vector2 wantedPosition = new Vector2(currentPosition.x, currentPosition.y - 45);
			while (timer <= 1)
			{
				timer += Time.deltaTime * 10;
				_loadFlightStructure._saveFileContent.anchoredPosition = Vector2.Lerp(
						currentPosition,
						wantedPosition,
						timer
					);
				yield return null;
			}
		}

		IEnumerator Coroutine_HideSaveFileInput()
		{
			inputToggled = false;
			float timer = 0;
			Vector2 currentPosition = _loadFlightStructure._saveFileContent.anchoredPosition;
			Vector2 wantedPosition = new Vector2(currentPosition.x, currentPosition.y + 45);
			while (timer <= 1)
			{
				timer += Time.deltaTime * 10;
				_loadFlightStructure._saveFileContent.anchoredPosition = Vector2.Lerp(
						currentPosition,
						wantedPosition,
						timer
					);
				yield return null;
			}
		}

		#endregion
	}

}