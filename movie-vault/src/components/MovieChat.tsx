import React, { useState } from 'react';
import { useMovieChat } from '../hooks/useMovieChat';4

export const MovieChat: React.FC = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [input, setInput] = useState('');
  const { messages, status, sendMessage } = useMovieChat('https://localhost:5001/moviechathub');

  const handleSend = () => {
    sendMessage(input);
    setInput('');
  };

  return (
    <>
      {/* 1. THE FLOATING ICON (Top Right) */}
      <button 
        onClick={() => setIsOpen(!isOpen)}
        className="fixed top-6 right-6 z-50 p-4 bg-blue-600 hover:bg-blue-700 text-white rounded-full shadow-2xl transition-transform hover:scale-110 active:scale-95"
      >
        {isOpen ? (
          <span className="text-xl font-bold">✕</span> // Close Icon
        ) : (
          <span className="text-xl">💬</span> // Chat Icon
        )}
      </button>

      {/* 2. THE CHAT WINDOW (Appears when isOpen is true) */}
      {isOpen && (
        <div className="fixed top-24 right-6 z-40 w-80 h-[500px] flex flex-col border border-gray-700 rounded-2xl shadow-2xl overflow-hidden bg-gray-900 animate-in fade-in slide-in-from-top-4 duration-300">
          <div className="bg-gray-800 p-4 border-b border-gray-700 flex justify-between items-center">
            <h3 className="font-bold text-white">Movie AI Assistant</h3>
            <span className={`text-xs ${status === 'Connected' ? 'text-green-400' : 'text-yellow-400'}`}>
              ● {status}
            </span>
          </div>

          <div className="flex-1 overflow-y-auto p-4 space-y-3 bg-gray-950">
            {messages.map((m) => (
              <div key={m.id} className={`max-w-[85%] p-3 rounded-xl text-sm ${
                m.sender === 'user' 
                ? 'bg-blue-600 text-white self-end ml-auto' 
                : 'bg-gray-800 text-gray-200 self-start'
              }`}>
                {m.text}
              </div>
            ))}
          </div>

          <div className="p-4 bg-gray-800 border-t border-gray-700 flex gap-2">
            <input 
              className="flex-1 bg-gray-700 border-none rounded-lg px-3 py-2 text-white text-sm focus:ring-2 focus:ring-blue-500 outline-none"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleSend()}
              placeholder="Ask me anything..."
            />
            <button onClick={handleSend} className="bg-blue-600 p-2 rounded-lg text-white">
              ➤
            </button>
          </div>
        </div>
      )}
    </>
  );
};