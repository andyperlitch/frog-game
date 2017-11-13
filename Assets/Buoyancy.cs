using UnityEngine;
using System.Collections;

public class Buoyancy : MonoBehaviour
{

  [SerializeField]
  string waterObjectName;

  [SerializeField]
  public bool highSurfaceTension;

  [SerializeField]
  public float buoyancyPointOffset = 0;


  public float underwaterDrag = 4f;
  public float UpwardForce = 12.72f; // 9.81 is the opposite of the default gravity, which is 9.81. If we want the boat not to behave like a submarine the upward force has to be higher than the gravity in order to push the boat to the surface
  private bool isInWater = false;
  private bool isSurfaced = false;
  private GameObject water;
  private float waterLevel;
  private Rigidbody rb;

  void Start() {
    rb = GetComponent<Rigidbody>();
    water = GameObject.Find(waterObjectName);
    Collider waterCollider = water.GetComponent<Collider>();
    waterLevel = water.transform.position.y + waterCollider.bounds.size.y / 2;
  }

  void OnTriggerEnter(Collider collider)
  {
    if (collider.gameObject.name == waterObjectName) {
      isInWater = true;
      rb.drag = underwaterDrag;
    }
  }

  void OnTriggerExit(Collider collider)
  {
    if (collider.gameObject.name == waterObjectName) {
      Debug.Log("Exiting water");
      if (highSurfaceTension)
      {
        isSurfaced = true;
      } else {
        isInWater = false;
      }
      rb.drag = 0.05f;
    }
  }

  void FixedUpdate()
  {
    if (isInWater) {
      if (isSurfaced) {
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
      } else if (transform.position.y + buoyancyPointOffset > waterLevel && rb.velocity.y > 0) {
        rb.AddForce(-Physics.gravity, ForceMode.Acceleration);
        isSurfaced = true;
      } else {
        // apply upward force
        Vector3 force = transform.up * UpwardForce;
        rb.AddRelativeForce(force, ForceMode.Acceleration);
        Debug.Log("Upward force: " + force + " @" + Time.time);
      }
    }
  }
}