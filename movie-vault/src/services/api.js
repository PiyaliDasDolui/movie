const API_URL = "http://localhost:5064/api/movies";

export const getPopularMovies = async () => {
  const response = await fetch(API_URL);
  if (!response.ok) throw new Error("Network response was not ok");
  return response.json();
};