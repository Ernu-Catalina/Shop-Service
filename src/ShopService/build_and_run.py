import subprocess
import os

# Use the absolute path to the folder that contains the .csproj
project_path = r"C:\Users\OSAdmin\OneDrive\UTM\Year_4\PAD\Shop\src\ShopService"

# Change working directory to the project folder
os.chdir(project_path)

# Restore, build, and run
subprocess.run(["dotnet", "restore", "ShopService.csproj"])
subprocess.run(["dotnet", "build", "ShopService.csproj"])
subprocess.run(["dotnet", "run", "--project", "ShopService.csproj", "--urls", "http://localhost:5000"])
