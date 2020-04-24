using UnityEngine;
namespace FlightRecorderPlugin {

	interface IFlightRecorder<T>
	{
		void PrepareDroneForPlaybackFlight(T obj);
	}

	public class FlightRecorderPlayback : MonoBehaviour, IFlightRecorder<DroneController.Physics.DroneMovementScript>
	{

		[HideInInspector] public bool Playbacking;
		[HideInInspector] public DroneMovement droneMovement;
		[HideInInspector] public float Vertical_W = 0;
		[HideInInspector] public float Vertical_S = 0;
		[HideInInspector] public float Horizontal_A = 0;
		[HideInInspector] public float Horizontal_D = 0;
		[HideInInspector] public float Vertical_I = 0;
		[HideInInspector] public float Vertical_K = 0;
		[HideInInspector] public float Horizontal_J = 0;
		[HideInInspector] public float Horizontal_L = 0;

		public virtual void Awake()
		{
			droneMovement = GetComponent<DroneMovement>();
		}

		public virtual void Start()
		{
			if(Playbacking)
				PrepareDroneForPlaybackFlight(droneMovement);
		}

		/// <summary>
		/// Starts playing recored flight.
		/// </summary>
		/// <param name="_callback">On finish playing recorded flight.</param>
		public void StartPlayback(System.Action _callback)
		{
			if (dataFromSaveFile.Length != 0)
			{
				FlightRecorder.Instance.StartPlayingPlayback(dataFromSaveFile, this, () =>
				{
					_callback();
					//on flight playback finished...
					//Debug.Log("Playback finished");
				});
			}
			else
			{
				Debug.LogError("•Playback data not loaded.", gameObject);
			}
		}

		public virtual void Update()
		{
			//If not recording, we want to store these values
			if (Playbacking == false)
			{
				Vertical_W = droneMovement.Vertical_W;
				Vertical_S = droneMovement.Vertical_S;
				Horizontal_A = droneMovement.Horizontal_A;
				Horizontal_D = droneMovement.Horizontal_D;
				Vertical_I = droneMovement.Vertical_I;
				Vertical_K = droneMovement.Vertical_K;
				Horizontal_J = droneMovement.Horizontal_J;
				Horizontal_L = droneMovement.Horizontal_L;
			}
			else
			{
				//playbacking is filling these info because drone is on custom input then...
				droneMovement.customFeed_forward = Vertical_W;
				droneMovement.customFeed_forward = Vertical_S;
				droneMovement.customFeed_forward = Horizontal_A;
				droneMovement.customFeed_forward = Horizontal_D;
				droneMovement.customFeed_forward = Vertical_I;
				droneMovement.customFeed_forward = Vertical_K;
				droneMovement.customFeed_forward = Horizontal_J;
				droneMovement.customFeed_forward = Horizontal_L;
			}
		}


		public string SaveFileName;
		[HideInInspector] public DataReadingStructure[] dataFromSaveFile;
		void DecodeSavedFile(string flightPath)
		{
			if (!FlightRecorder.Instance) //if load runtime
			{
				dataFromSaveFile = FindObjectOfType<FlightRecorder>().ReadRecodedFile(flightPath);
			}
			else //if load from editor
			{
				dataFromSaveFile = FlightRecorder.Instance.ReadRecodedFile(flightPath);
			}
		}

		public void LoadFlight(string flightPath)
		{
			string[] flightPathSplit = flightPath.Split('\\');
			SaveFileName = flightPathSplit[flightPathSplit.Length - 1];
			DecodeSavedFile(flightPath);
			Playbacking = (SaveFileName.Length > 0) ? true : false;
		}

		public void UnloadFlight()
		{
			SaveFileName = string.Empty;
			dataFromSaveFile = new DataReadingStructure[0];
			Playbacking = false;
		}

		public void PrepareDroneForPlaybackFlight(DroneController.Physics.DroneMovementScript obj)
		{
			//Debug.Log("Preparing my drone");
			obj.ourDrone.isKinematic = true;
			obj.ourDrone.useGravity = false;
			obj.customFeed = true;
			obj.inputEditorSelection = 0;
			obj.FlightRecorderOverride = true;
			obj.animatedGameObject.enabled = false;
		}
	}
}