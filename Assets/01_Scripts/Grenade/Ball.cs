using UnityEngine;

public class Ball : MonoBehaviour {
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private AudioSource _source;
    [SerializeField] private AudioClip[] _clips;
    [SerializeField] private GameObject _poofPrefab;
    private bool _isGhost;

    [SerializeField] private float _time = 5f;

    [SerializeField] private float _timer = 0f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float sphereCastRadius = 1f;
    [SerializeField] private bool explo = false;
    public int numberOfRaycasts = 36;

    public void Init(Vector3 velocity, bool isGhost) {
        _isGhost = isGhost;
        _rb.AddForce(velocity, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision col) {
        //if (_isGhost) return;
        //Instantiate(_poofPrefab, col.contacts[0].point, Quaternion.Euler(col.contacts[0].normal));
        //_source.clip = _clips[Random.Range(0, _clips.Length)];
        //_source.Play();
    }

    public void Start()
    {
        _timer = 0;
    }

    private void Update()
    {
        _timer += Time.deltaTime;

        if (_timer >= _time && explo == false)
        {
            Debug.Log("Booooooooooom tes mort pitot de merde t'es trop eclater au sol");
            ExplodeV2();
            explo = true;
        }
    }

    void Explode()
    {
        // SphereCast dans toutes les directions
        for (int i = 0; i < 360; i++)
        {
            Vector3 castDirection = Quaternion.Euler(0, i, 0) * transform.forward;
            RaycastHit hitInfo;

            if (Physics.SphereCast(transform.position, sphereCastRadius, castDirection, out hitInfo, explosionRadius))
            {
                // Si la SphereCast touche quelque chose, vous pouvez traiter l'objet touché ici
                Debug.Log("Collision détectée avec : " + hitInfo.collider.gameObject.name);

                // Faites quelque chose avec l'objet touché, par exemple :
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // Faire des dégâts au joueur, déclencher une animation, etc.
                    Debug.Log("Pitot rend les 10e sinon je tue fanny");
                }
            }

            // Dessin de la SphereCast dans l'éditeur pour le débogage
            Debug.DrawRay(transform.position, castDirection * explosionRadius, Color.red, 20f);
        }
    }

    void ExplodeV2()
    {
        // Calcul des raycasts autour de la sphère
        for (int i = 0; i < numberOfRaycasts; i++)
        {
            float angle = (i / (float)numberOfRaycasts) * 360f;
            float x = Mathf.Sin(angle * Mathf.Deg2Rad);
            float z = Mathf.Cos(angle * Mathf.Deg2Rad);

            Vector3 rayDirection = new Vector3(x, 0f, z);

            RaycastHit hitInfo;
            if (Physics.Raycast(transform.position, rayDirection, out hitInfo, explosionRadius))
            {
                // Si le raycast touche quelque chose, vous pouvez traiter l'objet touché ici
                Debug.Log("Collision détectée avec : " + hitInfo.collider.gameObject.name);

                // Faites quelque chose avec l'objet touché, par exemple :
                if (hitInfo.collider.CompareTag("Player"))
                {
                    // Faire des dégâts au joueur, déclencher une animation, etc.
                    Debug.Log(" t'es morts");
                }
            }

            // Dessin du raycast dans l'éditeur pour le débogage
            Debug.DrawRay(transform.position, rayDirection * explosionRadius, Color.red, 10f);
        }

        Destroy(gameObject);
    }
}

