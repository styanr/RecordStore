import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'formats';

class FormatService {
  getFormats = async (name) => {
    const response = await axios.get(api_url, { params: { name } });
    return response.data;
  };
}

const formatService = new FormatService();
export default formatService;
