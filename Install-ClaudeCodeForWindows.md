https://claude.ai/public/artifacts/5a119e06-02a1-4810-8681-ac7b1cf28e3e

wsl --install

<#
Set Unix User hug
Pwd: Plus-Landmine-Driller-Deferral0-Alone

Create a default Unix user account: hug
New password: 
Retype new password: 
passwd: password updated successfully
To run a command as administrator (user "root"), use "sudo <command>".
See "man sudo_root" for details.

#>

# In der Ubuntu Shell

sudo apt update && sudo apt upgrade -y

sudo apt remove nodejs npm

sudo apt update
sudo apt install curl build-essential -y

curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.7/install.sh | bash

source ~/.bashrc

command -v nvm

nvm install --lts
nvm use --lts
nvm alias default lts/*

mkdir ~/.npm-global

npm config set prefix '~/.npm-global'

echo 'export PATH=~/.npm-global/bin:$PATH' >> ~/.bashrc
source ~/.bashrc

npm install -g @anthropic-ai/claude-code