import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'genres';

class GenreService {
  getGenres = async (name) => {
    const response = await axios.get(api_url, { params: { name } });
    return response.data;
  };
}

const genreService = new GenreService();
export default genreService;
