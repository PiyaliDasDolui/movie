import { useState } from 'react';
import { useMutation, useQueryClient } from '@tanstack/react-query';
import { createMovie } from '../services/api';

export const CreateMovie = () => {
  const [title, setTitle] = useState('');
  const [posterPath, setPosterPath] = useState('');
  const [releaseDate, setReleaseDate] = useState('');
  const [overview, setOverview] = useState('');

  const queryClient = useQueryClient();
  const mutation = useMutation(createMovie, {
    onSuccess: () => {
      queryClient.invalidateQueries(['movies']);
      setTitle(''); setPosterPath(''); setReleaseDate(''); setOverview('');
    },
  });

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    mutation.mutate({ title, posterPath, releaseDate, overview });
  };

  return (
    <div className="container mx-auto p-6">
      <h2 className="mb-4 text-xl font-semibold text-white">Add New Movie</h2>
      <form onSubmit={handleSubmit} className="grid gap-3 sm:grid-cols-2">
        <input className="p-2 rounded bg-gray-800 border border-gray-700" placeholder="Title" value={title} onChange={e => setTitle(e.target.value)} required />
        <input className="p-2 rounded bg-gray-800 border border-gray-700" placeholder="Poster URL" value={posterPath} onChange={e => setPosterPath(e.target.value)} />
        <input className="p-2 rounded bg-gray-800 border border-gray-700" placeholder="Release Date" value={releaseDate} onChange={e => setReleaseDate(e.target.value)} />
        <input className="p-2 rounded bg-gray-800 border border-gray-700" placeholder="Overview" value={overview} onChange={e => setOverview(e.target.value)} />
        <div className="sm:col-span-2">
          <button type="submit" className="px-4 py-2 bg-blue-500 rounded hover:bg-blue-600" disabled={mutation.isLoading}>
            {mutation.isLoading ? 'Saving...' : 'Add Movie'}
          </button>
          {mutation.isError && (
            <div className="mt-2 text-red-400">Error: {(mutation.error as Error).message}</div>
          )}
          {mutation.isSuccess && (
            <div className="mt-2 text-green-400">Movie added successfully</div>
          )}
        </div>
      </form>
    </div>
  );
};

export default CreateMovie;
