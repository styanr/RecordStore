import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'admin/logs';

const useLogs = () => {
  const getLogs = async (params) => {
    const response = await axios.get(api_url, { params });
    return response.data;
  };

  return {
    getLogs,
  };
};

export default useLogs;
