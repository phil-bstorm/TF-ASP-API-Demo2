# Web API Demo 2

Cette démo a été réaliser pendant le cours de Web API (18/02/25).

Elle a pour but de réaliser une API REST en utilisant le framework ASP .NET Core avec le template Web API, une base de données SQLServer et Entity Framework Core.

## Table des matières

- [Web API Demo 2](#web-api-demo-2)
  - [Table des matières](#table-des-matières)
  - [Vocabulaires et acronymes](#vocabulaires-et-acronymes)
  - [Organisation de la solution](#organisation-de-la-solution)
  - [Explication du contenu de la démo](#explication-du-contenu-de-la-démo)
    - [Domain](#domain)
    - [DAL](#dal)
    - [BLL](#bll)
    - [API](#api)
  - [Rappel sur les verbes HTTP (Get, Post, Put, Patch, Delete)](#rappel-sur-les-verbes-http-get-post-put-patch-delete)
  - [Rappel sur l'injection de dépendance](#rappel-sur-linjection-de-dépendance)
    - [On prépare la dépendance (le service)](#on-prépare-la-dépendance-le-service)
    - [On met à disposition notre dépandance (service)](#on-met-à-disposition-notre-dépandance-service)
    - [Injecter la dépendance (service)](#injecter-la-dépendance-service)
  - [Encryption de mot de passe](#encryption-de-mot-de-passe)
    - [Bibliothèques](#bibliothèques)
    - [Utilisation](#utilisation)
  - [CORS](#cors)
    - [Configuration](#configuration)
  - [Envoie d'email (MailKit)](#envoie-demail-mailkit)
    - [SMTP4Dev](#smtp4dev)
      - [Installation](#installation)
      - [Interfaces](#interfaces)
    - [Mailkit](#mailkit)
      - [Installation](#installation-1)
      - [Utilisation](#utilisation-1)
        - [Interface](#interface)
        - [Service](#service)
        - [Mise à disposition de la dépendance](#mise-à-disposition-de-la-dépendance)
        - [Utilisation](#utilisation-2)
  - [Pagination](#pagination)

## Vocabulaires et acronymes

- API Rest: une application(serveur) qui utilise le protocole HTTP pour intéragir avec d'autres applications (clients) en utilisant des méthodes (GET, POST, PUT, PATCH, DELETE)
- CRUD: Create, Read, Update, Delete. Les 4 opérations de base pour intéragir avec une base de données, création, lecture, mise à jour et suppression
- ORM: Object-Relational Mapping, package qui permet de faire le lien entre les classes de l'application et la base de données (ex: Entity Framework Core)
- DTO: Data Transfer Object, classe qui permet de transférer des données entre le client et le serveur (ex: pour éviter de renvoyer des mots de passe ou caché des informations), ce qui peut permettre de réduire les données transmisse sur la bande passante
- JWT: JSON Web Token, token qui permet de sécuriser les échanges entre le client et le serveur (ex: pour s'authentifier)
- CORS: Cross-Origin Resource Sharing, permet de définir qui peut accéder à l'API (ex: pour éviter les attaques CSRF) _"définir qui peut accéder"_ = quelle URL peut accéder à l'API
- DI: Dependency Injection, permet de définir les dépendances d'une classe (ex: pour injecter un service dans un controller)
- Middleware: permet de faire des opérations avant ou après une requête (ex: gérer les exceptions, vérification de JWT, CORS...)

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

## Rappel sur les verbes HTTP (Get, Post, Put, Patch, Delete)

Chaque verbe à utiliser dans une API REST correspond à une action à réaliser sur une ressource.
_Note, il est possible de tout faire avec Post mais ce n'est VRAIMENT pas une bonne pratique._

- Get: récupérer des données (ex: récupérer tout les utilisateurs ou récupérer 1 utilisateur via son ID)
- Post: ajouter des données (ex: ajouter un utilisateur)
- Put: mettre à jour des données (ex: mettre à jour un utilisateur via son ID)
- Patch: mettre à jour partiellement des données (ex: mettre à jour le nom d'un utilisateur via son ID)
- Delete: supprimer des données (ex: supprimer un utilisateur via son ID)

## Rappel sur l'injection de dépendance

Personnelement, je séparer l'injection de dépendance en 3 étapes:

- Préparer la dépendance
- Mettre à disposition la dépendance
- Utliser (Injecter) la dépendance

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

## CORS

CORS (Cross-Origin Resource Sharing) permet de définir qui peut accéder à l'API.

C'est une sécurité qui permet d'éviter que n'importe qui (n'importe quelle URL) puisse accéder à l'API. (ex: éviter les attaques CSRF)

### Configuration

Tout d'abord, il faut ajouter le service CORS dans le fichier `Program.cs` entre `WebApplication.CreateBuilder` et `builder.Build`.

```csharp
builder.Services.AddCors(options =>
    {
        // pas de sécurité, tout le monde peut accèder à l'API.
        // utile pour le développement mais à NE PAS UTILISER EN PRODUCTION
        options.AddPolicy("FFA", policy => {
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });

        // Configuration de sécurité pour le développement
        // uniquement les clients avec ces URLs spécifiques peuvent accèder à l'API
        options.AddPolicy("Dev", policy =>
        {
            policy.WithOrigins("http://localhost:63342", "http://demo.be");
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
    }
);
```

Ensuite, il faut ajouter le middleware CORS dans le fichier `Program.cs`, après la redirection https (`app.UseHttpsRedirection();`)

```csharp
app.UseCors("Dev"); // on utilise la policy "Dev" définie plus haut
```

## Envoie d'email (MailKit)

Pour envoyer des emails, il est recommendé de passer par des services tiers (ex: SendGrid, MailJet, MailGun, etc...) cependant, ces solutions ne sont pas toujours gratuite.

Pour le développement, nous allons utiliser "SMTP4Dev" qui est une application qui permet de simuler un serveur SMTP. [https://github.com/rnwood/smtp4dev](https://github.com/rnwood/smtp4dev)

Et pour envoyer des emails, nous allons utiliser la bibliothèque `MailKit` qui permet d'envoyer des emails en utilisant le protocole SMTP.

### SMTP4Dev

#### Installation

Dans les "releases" (colonnes de droite), plusieurs options seront disponibles.

Pour ma part, j'utilise Windows 11, j'ai donc télécharger la version "Windows x64 binary standalone - Desktop app edition".

_Note: il existe une version "Desktop app edition" et "Server edition", les 2 fonctionnes très bien, la seule différences est que si vous utiliser la version serveur, il faudra se rendre la page web "http://localhost:5000" pour avoir accès à l'application_

#### Interfaces

Dans l'onglet `Message`, on retrouvera tout les mails reçus (à gauche) et le contenu du mail sélectionner (à droite). _Actuellement, vide car aucun mail n'a circulé via notre SMTP_.

En haut à droite de l'application, on notera qu'il est inscrit "SMTP server listening on port 25". Cela signifie que lors que nous devrons renseigner les informations de connection dans notre code, le `host` sera égale à `localhost` (ou `127.0.0.1`) et le `port` sera égale à `25`.

### Mailkit

Maintenant que notre serveur SMTP est mit en place, nous allons pouvoir envoyer des emails en utilisant la bibliothèque `MailKit`. L'avantage d'utiliser `MailKit` est qu'il va gérer pour nous la connection au serveur SMTP, l'envoie de l'email, etc...

#### Installation

Dans une API avec une architecture en N-Tier, l'envoie d'email sera gera dans le layer de controller (API).

Pour installer `MailKit`, il faut passer par le gestionnaire de NuGet package (clique droit sur le projet > `Gérer les packages NuGet...` > Parcourir > chercher `MailKit` > sélectionner le package `Mailkit` par jstedfast > Installer)

#### Utilisation

Dans le projet API, on va créer une dépendance pour le service d'envoie d'email.

##### Interface

Dans la démo: DemoAPI > Services > Interfaces > `IMailHelperService.cs`, notre `MailHelperService` servira à envoyer un mail lors de l'enregistrement du user et lors de sa connexion.

```csharp
using DemoAPI.Domain.Models;

namespace DemoAPI.Services.Interfaces
{
    public interface IMailHelperService
    {
        void SendWelcomeMail(Utilisateur utilisateur);
        void SendWarningLoginMail(Utilisateur utilisateur);
    }
}
```

##### Service

Dans le fichier de configuration (`appsettings.json`), nous avons renseigné différentes informations sous l'objet `Smtp`.

- `NoReply`: objet qui contient le nom et l'email de l'expéditeur
  - `Name`: nom de l'expéditeur
  - `Email`: email de l'expéditeur
- `Host`: adresse du serveur SMTP (`localhost` ou `127.0.0.1` si SMTP4Dev tourne en local)
- `Port`: port du serveur SMTP (`25` si SMTP4Dev tourne en local)

```csharp
using DemoAPI.Domain.Models;
using DemoAPI.Services.Interfaces;
using MailKit.Net.Smtp;
using MimeKit;
using MimeKit.Text;

namespace DemoAPI.Services
{
    public class MailHelperService : IMailHelperService
    {
        private readonly string _noReplyName;
        private readonly string _noReplyEmail;
        private readonly string _smtpHost;
        private readonly int _smptPort;

        public MailHelperService(IConfiguration configuration)
        {
            // récupérer les informations de fichier de configuration
            // Expéditeur
            _noReplyName = configuration.GetValue<string>("Smtp:NoReply:Name")!;
            _noReplyEmail = configuration.GetValue<string>("Smtp:NoReply:Email")!;
            // Serveur SMTP
            _smtpHost = configuration.GetValue<string>("Smtp:Host")!;
            _smptPort = configuration.GetValue<int>("Smpt:Port");
        }

        private SmtpClient GetSmtpClient()
        {
            // connection vers le SMTP
            SmtpClient client = new SmtpClient();
            client.Connect(_smtpHost, _smptPort);
            // client.Authenticate(...); // si besoin d'une authentification avec le server SMTP
            return client;
        }

        public void SendWelcomeMail(Utilisateur utilisateur)
        {
            // préparation de l'email
            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(_noReplyName, _noReplyEmail));
            email.To.Add(new MailboxAddress(utilisateur.Username, utilisateur.Email));
            email.Subject = "Bienvenue sur notre super site !";
            email.Body = new TextPart(TextFormat.Plain)
            {
                Text = $"Bienvenue dans notre site, {utilisateur.Username}! \n\n" +
                        "╰(*°▽°*)╯ \n\n" +
                        "Coordialement l'équipe Demo."
            };

            // connection vers le SMTP
            using SmtpClient client = GetSmtpClient();

            // envoie de l'email
            client.Send(email);

            // déconnection du SMTP
            client.Disconnect(true);
        }

        public void SendWarningLoginMail(Utilisateur utilisateur)
        {
            // préparation de l'email
            MimeMessage email = new MimeMessage();
            email.From.Add(new MailboxAddress(_noReplyName, _noReplyEmail));
            email.To.Add(new MailboxAddress(utilisateur.Username, utilisateur.Email));
            email.Subject = "Attention une connection à votre compte a été detectée";
            email.Body = new TextPart(TextFormat.Html)
            {
                Text = $"<!DOCTYPE html><html lang=\"fr\">" +
                $"<body>" +
                $"<p>Hello world</p>" +
                $"</body>" +
                $"</html>"
            };

            // connection vers le SMTP
            using SmtpClient client = GetSmtpClient();

            // envoie de l'email
            client.Send(email);

            // déconnection du SMTP
            client.Disconnect(true);
        }
    }
}
```

##### Mise à disposition de la dépendance

Cf: [Rappel sur l'injection de dépendance](#rappel-sur-linjection-de-dépendance)

##### Utilisation

Dans le controller, on va utiliser le service pour envoyer un email lors de l'enregistrement d'un utilisateur.

```csharp
[HttpPost]
public ActionResult<DetailsUtilisateurDTO> Create([FromForm] CreateUtilisateurDTO dto) {
    if (ModelState.IsValid)
    {
        // return _utilisateurService.Create(dto.ToUtilisateur()).ToDetailsUtilisateurDTO();
        Utilisateur utilisateur = dto.ToUtilisateur();
        Utilisateur updated = _utilisateurService.Create(utilisateur);

        _mailHelperService.SendWelcomeMail(utilisateur); // envoie de l'email de bienvenue

        return Ok(updated.ToDetailsUtilisateurDTO());
    }

    return BadRequest();
}
```

## Pagination

La pagination permet de limiter le nombre de résultats renvoyés par une requête.

Par exemple, si on a 1000 utilisateurs dans la base de données, il est inutile de tous les renvoyer en une seule fois. Il est préférable de les renvoyer par groupe de 10, 20, 50, 100, etc...

Dans la démo, lorsqu'on a implémenté la pagination, j'ai commencé par la DAL, puis la BLL et enfin l'API.

⚠️ Note: dans l'exemple de code dans la démo, notre pagination commence à ZÉRO. Si on souhaite commencer à 1, il faudra ajouter `1` à la variable `Page` dans la BLL. (car c'est de la logique métier)

### DAL

Dans la DAL, dans les `repositories` qui possède une méthode `GetAll`, on va ajouter 2 paramètres:

- `int offset`: le nombre de résultats à sauter
- `int limit`: le nombre de résultats à renvoyer

_La raiosn pour la quelle je parle avec `offset` et `limit`, c'est pour garder le même "vocabulaire" que la base de donnée._

```csharp
public IEnumerable<Car> GetAll(int offset = 0, int limit = 20)
{
    return _context.Cars
        .Skip(offset) // on utilise 'Skip' de EF Core pour ignoré les premiers résultats
        .Take(limit); // on utilise 'Take' de EF Core pour limiter le nombre de résultats
}
```

### BLL

Dans la BLL, on va créer un nouveau model `PaginationParam` qui va contenir les informations de pagination.

```csharp
namespace DemoAPI.Domain.Models
{
    public class PaginationParams
    {
        public int Page { get; set; } = 0; // page actuel (attention on démarre de zéro, comme dans un tableau)
        public int PageSize { get; set; } = 3; // nombre de résultats par page
    }
}
```

Dans les services, on va ajouter un paramètre `PaginationParams` à la méthode `GetAll`.

Et vu que notre repository utilise `offset` et `limit`, on va devoir faire la conversion entre `Page` et `PageSize` en `offset` et `limit`.

```csharp
public IEnumerable<Utilisateur> GetAll(PaginationParams pagination)
{
    int offset = pagination.Page * pagination.PageSize; // conversion de la page en offset
    int limit = pagination.PageSize;

    // on retourne maximum 100 résultats
    if(limit > 100)
    {
        limit = 100;
    }

    return _utilisateurRepository.GetAll(offset, limit);
}
```

_Note: ici on rajoute une règle métier qui fait qu'on ne retournera pas plus de 100 résultats par requête_

### API

Dans l'API, on va ajouter un paramètre `PaginationParams` à la méthode `GetAll`.

```csharp
[HttpGet]
public ActionResult<IEnumerable<ListUtilisateurDTO>> GetAll([FromQuery] PaginationParams pagination) {
    IEnumerable<Utilisateur> utilisateurs = _utilisateurService.GetAll(pagination);

    IEnumerable<ListUtilisateurDTO> usersDTO = utilisateurs.Select(u => u.ToListUtilisateurDTO());

    return Ok(usersDTO);
}
```

Ici on précise que la pagination proviens des variables qui se trouve dans les Query Parameters de la requête HTTP. (Query Parameters = `?page=0&pageSize=3` à la suite de l'url, ex: `https://localhost:7201/Utilisateur?Page=1&PageSize=4`)

_Note: il est aussi recommencer de transformer `PaginationParams` en DTO mais étandonné que je ne souhaite pas de validation dans cette classe (pour le moment car ce n'était pas le but de cet exemple), je n'ai pas créé de DTO_
