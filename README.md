AI Chat
Det här projektet är en AI-baserad chattapplikation som är byggd med React, TypeScript och ett .NET Minimal API. Applikationen använder Azure AI Foundry för den generativa AI-delen och SQLite tillsammans med Entity Framework Core för att spara chattar och meddelanden. Syftet med projektet är att skapa en enkel chattapplikation där användaren kan skapa flera olika konversationer och fortsätta på tidigare chattar.


För att starta projektet behöver både backend och frontend köras. För backend går man till projektmappen och kör dotnet restore och sedan dotnet run. API:t körs lokalt på den port som anges i terminalen, exempelvis http://localhost:5068. Frontend startas genom att gå till frontend-mappen och köra npm install följt av npm run dev. React-applikationen körs då normalt på http://localhost:5173.

!!!!!!!!!!!!OBS Av någon anledning vart det dubbelmapp i frontend så man måste navigera dit två gånger 



Azure API-nyckeln ska inte finnas direkt i projektets källkod eftersom den då riskerar att delas tillsammans med projektet. Därför används .NET User Secrets för att lagra Azure-informationen lokalt under utvecklingen.

User Secrets initieras med dotnet user-secrets init och därefter läggs endpoint och API-nyckel till med dotnet user-secrets set. På så sätt kan projektet använda Azure utan att själva nyckeln behöver läggas upp på exempelvis GitHub.


Programmet fungerar genom att användaren först kan skapa en ny chatt. Chatten sparas då i SQLite-databasen. När användaren skriver ett meddelande skickar React det till .NET API:t. 


API:t sparar användarens meddelande i databasen och hämtar sedan tidigare meddelanden från samma chatt. Dessa meddelanden skickas vidare till den generativa AI-modellen i Azure tillsammans med det nya meddelandet. AI:n genererar sedan ett svar som skickas tillbaka till API:t. Svaret sparas också i databasen och skickas därefter tillbaka till React där det visas i chatten. Eftersom tidigare meddelanden skickas med till AI:n kan modellen använda kontext från den aktuella konversationen.


Applikationen har även stöd för flera olika chattar. Användaren kan skapa nya chattar, välja mellan befintliga chattar, se tidigare meddelanden och radera chattar. Den första frågan i en ny chatt används automatiskt som chattens titel, vilket gör det lättare att skilja mellan olika konversationer. Frontend-gränssnittet är byggt i React och använder bland annat state för att hålla reda på chattar, meddelanden, vilken chatt som är vald och om AI:n håller på att generera ett svar.


Den AI-tjänst som används i projektet är GPT-4.1-mini via Azure AI Foundry. Den valdes eftersom den fanns tillgänglig i projektets Azure-miljö och eftersom den lämpar sig för generativ text och konversationsbaserade svar. Modellen används för att generera nya svar baserat på användarens meddelanden och historiken från samma chatt. På så sätt används generativ AI som en central del av applikationen istället för att svaren är förutbestämda eller hårdkodade.
Projektet består av en backend där Program.cs hanterar API-endpoints, AiService ansvarar för kommunikationen med Azure AI och AppDbContext hanterar databasen genom Entity Framework Core. Det finns modeller för bland annat chattar och meddelanden. Frontenden består av React-komponenter och en separat service för API-anrop. På så sätt är ansvaret uppdelat mellan frontend, API, databas och AI-tjänsten.