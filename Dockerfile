FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY *.sln .
COPY src/UPictures.Scanner/*.csproj ./src/UPictures.Scanner/
COPY src/UPictures.Web/*.csproj ./src/UPictures.Web/
RUN dotnet restore

# copy everything else and build app
COPY src/. ./src/
WORKDIR /app/src/UPictures.Web
RUN dotnet publish -c Release -o out


FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
COPY --from=build /app/src/UPictures.Web/out ./
ENTRYPOINT ["dotnet", "UPictures.Web.dll"]