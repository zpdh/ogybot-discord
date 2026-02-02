# Use the official .NET SDK 8.0 image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS bot-build

# Set the working directory inside the container
WORKDIR /usr/local/bot

# Copy all the files from the current directory to the container
COPY ./ ./
COPY ./ogybot.Bot/.env ./ogybot.Bot/appsettings.json

# Restore dependencies for the project
RUN dotnet restore ogybot.Bot/ogybot.Bot.csproj

# Build the project in Release mode
RUN dotnet build ogybot.Bot/ogybot.Bot.csproj -c Release -o /usr/local/bot/build

# Publish the project into a folder optimized for deployment
RUN dotnet publish ogybot.Bot/ogybot.Bot.csproj -c Release -o /usr/local/bot/publish --no-restore

# Use the runtime-only .NET 8.0 image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS bot-final

# Set the working directory in the runtime container
WORKDIR /usr/local/bot

# Copy the build artifacts from the build stage
COPY --from=bot-build /usr/local/bot/publish ./

# Set the entry point for the container
ENTRYPOINT ["dotnet", "ogybot.Bot.dll"]