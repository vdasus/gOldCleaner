dotnet sonarscanner begin /k:"gOldCleaner" /d:sonar.host.url="http://localhost:9000" /d:sonar.login="43283aee248e024c451d23975a6aaad9777ac2b2"
dotnet build gOldCleaner.sln 
dotnet sonarscanner end /d:sonar.login="43283aee248e024c451d23975a6aaad9777ac2b2"