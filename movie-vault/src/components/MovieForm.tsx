import React, { useState } from 'react';

interface MovieFormProps {
  onAdd: (movie: any) => void;
  onClose: () => void;
}

const MovieForm: React.FC<MovieFormProps> = ({ onAdd, onClose }) => {
  const [formData, setFormData] = useState({
    title: '',
    poster_path: '',
    vote_average: '',
    release_date: '',
    overview: ''
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    // Format the data to match your Movie interface
    const newMovie = {
      ...formData,
      id: Date.now(), // Temporary ID
      vote_average: parseFloat(formData.vote_average) || 0,
      poster_path: formData.poster_path || 'https://via.placeholder.com/500x750?text=No+Image'
    };
    onAdd(newMovie);
    onClose();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-75 flex justify-center items-center z-50 p-4">
      <div className="bg-gray-900 p-8 rounded-lg w-full max-w-md border border-gray-700">
        <h2 className="text-2xl font-bold text-white mb-4">Add New Movie</h2>
        <form onSubmit={handleSubmit} className="space-y-4">
          <input
            className="w-full p-2 rounded bg-gray-800 text-white border border-gray-600"
            placeholder="Movie Title"
            required
            onChange={(e) => setFormData({...formData, title: e.target.value})}
          />
          <input
            className="w-full p-2 rounded bg-gray-800 text-white border border-gray-600"
            placeholder="Poster Image URL"
            onChange={(e) => setFormData({...formData, poster_path: e.target.value})}
          />
          <input
            type="number" step="0.1" max="10"
            className="w-full p-2 rounded bg-gray-800 text-white border border-gray-600"
            placeholder="Rating (0-10)"
            onChange={(e) => setFormData({...formData, vote_average: e.target.value})}
          />
          <textarea
            className="w-full p-2 rounded bg-gray-800 text-white border border-gray-600"
            placeholder="Overview"
            rows={3}
            onChange={(e) => setFormData({...formData, overview: e.target.value})}
          />
          <div className="flex gap-4">
            <button type="submit" className="flex-1 bg-red-600 text-white py-2 rounded hover:bg-red-700 transition">Save</button>
            <button type="button" onClick={onClose} className="flex-1 bg-gray-600 text-white py-2 rounded hover:bg-gray-700 transition">Cancel</button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default MovieForm;