import { useEffect, useState, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { ChatMessage } from '../types';

export const useMovieChat = (hubUrl: string) => {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [status, setStatus] = useState<'Connected' | 'Connecting' | 'Error'>('Connecting');
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    // 1. Setup connection
    const connection = new signalR.HubConnectionBuilder()
      .withUrl(hubUrl)
      .withAutomaticReconnect()
      .build();

    connectionRef.current = connection;

    // 2. Listen for "tokens" from the .NET Streaming AI
    connection.on("ReceiveToken", (token: string) => {
      setMessages(prev => {
        const lastMsg = prev[prev.length - 1];
        if (lastMsg?.sender === 'ai') {
          return [...prev.slice(0, -1), { ...lastMsg, text: lastMsg.text + token }];
        }
        return [...prev, { id: crypto.randomUUID(), sender: 'ai', text: token, timestamp: new Date() }];
      });
    });

    // 3. Start connection
    connection.start()
      .then(() => setStatus('Connected'))
      .catch(() => setStatus('Error'));

    return () => { connection.stop(); }; // Cleanup
  }, [hubUrl]);

  const sendMessage = async (text: string) => {
    if (connectionRef.current && text.trim()) {
      const userMsg: ChatMessage = { id: crypto.randomUUID(), sender: 'user', text, timestamp: new Date() };
      setMessages(prev => [...prev, userMsg]);
      await connectionRef.current.invoke("AskAssistant", text);
    }
  };

  return { messages, status, sendMessage };
};