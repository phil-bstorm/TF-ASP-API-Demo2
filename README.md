# Web API Demo 2

Cette démo a été réaliser pendant le cours de Web API (18/02/25).

Elle a pour but de réaliser une API REST en utilisant le framework ASP .NET Core avec le template Web API, une base de données SQLServer et Entity Framework Core.

## Organisation de la solution

Cette solution a utilise l'architecture suivante: N-Tiers (ou modèle en couche).

- **DemoAPI**: contient les controllers
- **DemoAPI.BLL**: contient la logique métier (qui actuellement ne fait que communiquer avec la DAL)
- **DemoAPI.DAL**: contient les intéraction avec la base de données
- **DemoAPI.Domain**: contient les classes utilisées à travers l'application

Utiliser une architecture N-Tier, permet de bien séparer les responsabilités de chaque niveau de l'application

- `API` (controller) ne s'occupe que de la communication avec le client (reçois et renvoie des données) (ex: reçois une requette HTTP et renvoie une réponse HTTP)
- `BLL` ne s'occupe que des règles métiers. (ex: l'utilisateur doit avoir au moins 18ans)
- `DAL` ne s'occupe que de la communication avec la base de données (ex: requête SQL)

## Explication du contenu de la démo

Les prochaines parties seront décrite dans l'ordre que l'on a implémenté les différentes parties de l'application.

### Domain

Dans le projet `Domain`, on va retrouver une class `Car` avec différentes propriétés

- `Id` : Identifiant unique auto-généré par la base de donnée (non-requis pour la création d'une nouvelle voiture)
- `Brand` : Marque de la voiture
- `Model` : Modèle de la voiture
- `Color`: Couleur de la voiture
- `HorsePower`: Puissance de la voiture
- `IsNew`: Indique si la voiture est neuve ou non (si non spécifié, la valeur par défaut est `true`)

```csharp
namespace DemoAPI.Domain
{
    public class Car
    {
        public int Id { get; set; }
        public required string Brand { get; set; }
        public required string Model { get; set; }
        public required string Color { get; set; }
        public required int HorsePower { get; set; }
        public bool IsNew { get; set; }
    }
}
```

⚠️ Note: il est nécessaire de spécifier `{ get; set;}`, le fait que les champs soient publique ne suffisent pas pour la sérialisation des objets en JSON.

### DAL

Le projet `DAL` utilise le pattern Repository pour intéragir avec la base de données.

Dans le projet `DAL`, on va retrouver plusieurs dossiers:

- `Database`: dossier qui contient le context et la configuration de la base de données (configurations des tables, colonnes... avec Entity Framework Core)
  - `Configurations`: dossier qui contient les différents configration des tables (ex: `CarConfig`)
  - `DemoContext`: classe qui hérite de `DbContext` et qui permet de faire le lien entre les classes du domaine et la base de données
- `Migrations`: dossier qui contient les migrations de la base de données (pour rappel : `add-migration <nom>` pour créer une migration et `update-database` pour appliquer les migrations)
- `Repositories`: dossier contient les classes qui vont intéragir avec la base de données (CRUD) et qui seront utiliser par la BLL.
  - `Interfaces`: dossier qui contient les interfaces des repositories
- `CarRepository`: classe qui implémente l'interface `ICarRepository` et qui permet de faire les opérations sur la table `Car` (actuellement `GetAll` pour récuperer toutes les voitures et `Create` pour ajouter une nouvelle voiture)

Note:

- Le projet DAL ne fait aucune vérification sur les données, il se contente de les envoyer à la base de données. (c'est la BLL qui doit faire les vérifications)
- On utilise de l'injection de dépandance pour injecter le contexte de la base de données dans les repositories.

### BLL

Le projet `BLL` contient la logique métier de l'application et utilise l'injection de dépendance pour intéragir avec le projet `DAL`.

Dans le projet `BLL`, on va retrouver un dossier:

- `Serivces`: dossier qui contient les services qui vont être utiliser par le controller pour faire les vérifications métiers et intéragir avec différents repositories si besoin.
  - `Interfaces`: dossier qui contient les interfaces des services
    - `IService`: interface qui contient les méthodes communes à tous les services (en utilisant de la généricité `<T>`)
    - `ICarService`: interface qui contient les méthodes spécifiques aux voitures (actuellement vide car les méthodes de `IService` suffisent)
  - `CarService`: classe qui implémente l'interface `ICarService` et qui contient les vérifications métiers (actuellement `Create` pour vérifier si la voiture est valide et l'ajouter à la base de données)

Note:

- Le projet BLL ne fait aucune intéraction avec la base de données, il se contente de faire les vérifications métiers et de faire appel aux repositories pour les opérations CRUD.
- On utilise de l'injection de dépandance pour injecter les repositories dans les services.

### API

Le projet `API` contient les controllers qui vont intéragir avec le client (recevoir et renvoyer des données) et utilise l'injection de dépendance pour intéragir avec le projet `BLL`.

Dans le projet `API`, on va retrouver plusieurs dossiers:

- `Propriétés`: dossier qui contient le fichier `launchSettings.json` qui permet de configurer le lancement de l'application
- `Controller`: dossier qui contient les controllers de l'application
  - `CarController`: controller qui contient les différentes routes pour intéragir avec les voitures (actuellement `GetAll` pour récuperer toutes les voitures et `Create` pour ajouter une nouvelle voiture)
- `appsettings.json`: fichier de configuration de l'application (ex: contient la connection string de la base de données)
- `Startup.cs`: fichier de configuration de l'application (ex: configuration des services, des middlewares...)

## Rappel sur l'injection de dépendance

### On prépare la dépendance (le service)

1. On crée une interface qui contient les méthodes que l'on veut utiliser
2. On implémente cette interface dans une classe

```csharp
namespace DemoAPI.DAL.Repositories
{
    public class CarRepository : ICarRepository
    {
        private readonly DemoDbContext _context;

        public CarRepository(DemoDbContext context)
        {
            _context = context;
        }

        public Car Create(Car car)
        {
            Car result = _context.Cars.Add(car).Entity;
            _context.SaveChanges();
            return result;
        }

        public IEnumerable<Car> GetAll()
        {
            return _context.Cars;
        }
    }
}
```

### On met à disposition notre dépandance (service)

Dans program.cs, on configure les services entre `WebApplication.CreateBuilder` et `builder.Build`.

```csharp
(...)

var builder = WebApplication.CreateBuilder(args);

(...)

builder.Services.AddScoped<ICarRepository, CarRepository>(); // mise à disposition de la dépendance (on lié l'interface `ICarRepository` à la classe `CarRepository`)
(...)

var app = builder.Build();

(...)
```

### Injecter la dépendance (service)

Dans un autre service:

```csharp
namespace DemoAPI.BLL.Services
{
    public class CarService : ICarService
    {
        private readonly ICarRepository _carRepository; // variable pour stocker la dépendance (private readonly pour ne pas qu'elle soit modifiée)

        public CarService(ICarRepository carRepository) // injection de la dépendance
        {
            _carRepository = carRepository; // sauvegarde de la dépendance dans la variable
        }

        public Car Create(Car entity)
        {
           Car added = _carRepository.Create(entity); // utilisations de la dépendance
            return added;
        }

        public IEnumerable<Car> GetAll()
        {
            IEnumerable<Car> cars = _carRepository.GetAll(); // utilisations de la dépendance
            return cars;
        }
    }
}
```

## Encryption de mot de passe

Pour éviter d'avoir le mot de passe en clair dans la base de données, il est nécessaire d'encrypter le mot de passe.

**Dans quel layer fait-on l'encryption du mot de passe?** L'encryption du mot de passe est souvent fait dans le layer BLL, cependant cela reste un choix personnel. Par exemple, il est possible de faire l'encryption dans le layer API pour que le mot de passe soit encrypté le plus tôt dans l'application ou encore lors de l'INSERT dans la base de données.

**Quand fait-on l'encryption du mot de passe?** L'encryption du mot de passe est fait lors de la création du utilisateur (ou lors de la modification du mot de passe).

Une fois le mot de passe encrypter dans la base de donnée, pour vérifier si un mot de passe correspond au hash de la base de données, il suffit d'utiliser la méthode `Verify` de la bibliothèque utilisée.

### Bibliothèques

Pour encrypter un mot de passe, plusieurs bibliothèques sont disponibles:

- BCrypt
- Argon2
- et bien d'autres

Dans cette démo, nous allons utilisé `Argon2` avec le Nugget package suivant: `Isopoh.Cryptography.Argon2`.

### Utilisation

On notera 2 méthodes principales:

- `Hash` : pour encrypter un mot de passe
- `Verify` : pour vérifier si un mot de passe correspond à un hash

Exemples d'encryption dans le UserService de la BLL:

```csharp
//...
using Isopoh.Cryptography.Argon2;
//...

public Utilisateur Create(Utilisateur entity)
{
    // vérifier que mon email n'est pas déjà en DB
    Utilisateur? existingEmail = _utilisateurRepository.GetByEmail(entity.Email);
    if (existingEmail is not null)
    {
        throw new Exception("Email already exists");
    }

    // encryption du mot de passe
    entity.Password = Argon2.Hash(entity.Password); // encryption du mot de passe et sauvegarde dans l'entité

    return _utilisateurRepository.Create(entity);
}
//...
```

Exemple de vérification dans le UserService de la BLL:

```csharp
//...
using Isopoh.Cryptography.Argon2;
//...

public Utilisateur? Login(string username, string password)
{
    Utilisateur? user = _utilisateurRepository.GetByUsername(username);
    if (user is null)
    {
        return null;
    }

    if (Argon2.Verify(user.Password, password)) // vérification du mot de passe
    {
        return user;
    }

    return null;
}
//...
```
