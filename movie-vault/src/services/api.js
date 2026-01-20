const API_URL = "http://localhost:5064/api/movies";

export const getPopularMovies = async () => {
  const response = await fetch(API_URL);
  if (!response.ok) throw new Error("Network response was not ok");
  return response.json();
};

export const createMovie = async (movie) => {
  const response = await fetch(API_URL, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(movie),
  });
  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || 'Failed to create movie');
  }
  return response.json();
};