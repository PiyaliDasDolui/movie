// hooks/useMovieChat.ts
import { useEffect, useState, useRef } from 'react';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

export interface Message {
  id: string;
  text: string;
  sender: 'user' | 'assistant';
}

export const useMovieChat = (hubUrl: string) => {
  const [messages, setMessages] = useState<Message[]>([]);
  const [status, setStatus] = useState('Disconnected');
  const connectionRef = useRef<HubConnection | null>(null);

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveToken", (token: string) => {
      setMessages(prev => {
        const lastMessage = prev[prev.length - 1];
        
        // If the last message is from the assistant, append the token
        if (lastMessage && lastMessage.sender === 'assistant') {
          const updatedLastMessage = { ...lastMessage, text: lastMessage.text + token };
          return [...prev.slice(0, -1), updatedLastMessage];
        } 
        
        // Otherwise, create a new assistant message entry
        return [...prev, { id: Date.now().toString(), text: token, sender: 'assistant' }];
      });
    });

    connection.start()
      .then(() => setStatus('Connected'))
      .catch(() => setStatus('Error'));

    connectionRef.current = connection;

    return () => { connection.stop(); };
  }, [hubUrl]);

  const sendMessage = async (text: string) => {
    if (connectionRef.current && status === 'Connected') {
      // Add user message to UI
      const userMsg: Message = { id: Date.now().toString(), text, sender: 'user' };
      setMessages(prev => [...prev, userMsg]);
      
      // Call the Hub method
      await connectionRef.current.invoke("AskAssistant", text);
    }
  };

  return { messages, status, sendMessage };
};