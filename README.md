# BitcoinMessagingApp
A Windows Forms application for secure blockchain-based messaging using Bitcoin Core.

This application allows users to send and receive messages through the Bitcoin blockchain. It leverages the Bitcoin Core RPC API for seamless communication and provides a simple and intuitive interface for interacting with the blockchain. Key features include:

Features

- Login System: Log in with your Bitcoin Core RPC credentials.
- Message Sending: Send messages to specific Bitcoin addresses using the blockchain's OP_RETURN functionality.
- Automatic Inbox Updates: Timer-based inbox checks to notify users of new messages.
- Error Handling: Comprehensive feedback for connection issues or other errors.
- User-Friendly Interface: Simple design with clear instructions and placeholders.

How It Works

- Login: Users enter their Bitcoin Core RPC URL, username, and password to connect.
- Send Messages: Write a message, specify the recipient address, and send it securely via the blockchain.
- Inbox: Automatically checks for new blockchain messages and displays updates in real time.

Requirements

- Bitcoin Core: Fully synchronized Bitcoin node with RPC enabled.
- .NET SDK: To build and run the Windows Forms application.
- Visual Studio Code (or any compatible IDE).

Getting Started

- Clone the repository:git clone https://github.com/yourusername/bitcoin-messaging-app.git

- Build the application:dotnet build

- Run the application:dotnet run




