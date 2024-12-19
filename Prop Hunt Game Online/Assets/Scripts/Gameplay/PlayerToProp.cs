using UnityEngine;

public class PlayerToProp : MonoBehaviour
{
    [SerializeField] private Transform modelParent; // El contenedor de modelos
    [SerializeField] private GameObject currentModel; // El modelo actual del jugador
    [SerializeField] private float transformDistance = 5f; // Distancia m�xima para transformarse
    [SerializeField] private LayerMask transformLayer; // Capa de los objetos transformables
    [SerializeField] public bool Hunter = false;
    public GameObject CaraterMesh;
    public GameObject Gun;

    public GameObject transformTarget; // Referencia al objeto en el que te transformas.
    private Collider originalCollider;  // Colisionador original del jugador.


    public SkinnedMeshRenderer Player_Renderer;
    public Material Material_Hunter, Material_Alien;

    private void Start()
    {
        PlayerTeam();
    }


    void Update()
    {

        // Dibujar un rayo desde el centro de la c�mara
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        RaycastHit hit;

        if (Input.GetKeyDown(KeyCode.T))
        {
            Hunter = !Hunter;
        }
        PlayerTeam();

        if (Hunter == true){
            CaraterMesh.layer = 6;
            CaraterMesh.SetActive(true);
            currentModel.SetActive(false);
        }
        // Para saver que tipo de player es
        if (Hunter == false)
        {

            CaraterMesh.layer = 7;
            // Verificar si el rayo impacta con un objeto en la capa transformable
            if (Physics.Raycast(ray, out hit, transformDistance, transformLayer))
            {
                Debug.Log("Apuntando a un objeto transformable: " + hit.collider.name);


                // Si presionamos 'F' nos transformamos en el objeto
                if (Input.GetKeyDown(KeyCode.F))
                {
                    TransformIntoObject(hit.collider.gameObject);
                }
            }
            // Si presionamos 'R' resetea el mash al inical
            if (Input.GetKeyDown(KeyCode.R))
            {
                CaraterMesh.SetActive(true);
                currentModel.SetActive(false);

                TransformPlayer();
            }
        }

        Consolas();

    }

    private void TransformIntoObject(GameObject targetObject)
    {
        if (targetObject == null)
        {
            Debug.LogWarning("El objeto objetivo es nulo. No se puede transformar.");
            return;
        }

        Debug.Log("Transform�ndose en: " + targetObject.name);

        // Destruir el modelo actual
        if (currentModel != null)
        {
            CaraterMesh.SetActive(false);
            Destroy(currentModel);
            Debug.Log("Modelo actual destruido.");
        }

        // Crear un nuevo modelo basado en el objetivo
        GameObject newModel = Instantiate(targetObject, modelParent);
        newModel.layer = 7;
        newModel.transform.localPosition = new Vector3(0f,0.6f,-0.5f);
        newModel.transform.localRotation = Quaternion.identity;

        // Actualizar la referencia del modelo actual
        currentModel = newModel;
        Debug.Log("Transformaci�n completada en: " + targetObject.name);
    }

    void TransformPlayer()
    {
        if (transformTarget != null)
        {
            // Cambiar el colisionador del jugador al colisionador del objeto de transformaci�n.
            CapsuleCollider playerCollider = GetComponent<CapsuleCollider>();
            Collider targetCollider = transformTarget.GetComponent<Collider>();

            if (targetCollider != null)
            {
                // Guardar el colisionador original.
                originalCollider = playerCollider;
                // Desactivar el colisionador actual.
                playerCollider.enabled = false;
                // A�adir el colisionador del objeto destino.
                transformTarget.GetComponent<Collider>().enabled = true;

                // Cambiar a la c�mara o control del objeto destino si es necesario.
            }
        }
    }

    //Mira la variable y canvia que pude hacer el player
    void PlayerTeam()
    {
        if (Hunter == false)
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Alien;
            Player_Renderer.sharedMaterials = materials;
            Gun.SetActive(false);
        }
        else
        {
            Material[] materials = new Material[Player_Renderer.sharedMaterials.Length];
            materials[0] = Material_Hunter;
            Player_Renderer.sharedMaterials = materials;
            Gun.SetActive(true);
        }
    }

    void Consolas()
    {
        //Funcionalidad de las difrenetes consolas
        if (Input.GetKeyDown(KeyCode.E))
        {
            float interact_Range = 2f;
            Collider[] collider_array = Physics.OverlapSphere(transform.position, interact_Range);
            foreach (Collider collider in collider_array)
            {
                if (collider.TryGetComponent(out ConsoletoHunter Console_Hunter))
                {
                    Console_Hunter.Interact();
                    Hunter = true;
                }
                if (collider.TryGetComponent(out ConsoletoAlien Console_Alien))
                {
                    Console_Alien.Interact();
                    Hunter = false;
                }
            }
        }     
    }

}
