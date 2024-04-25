import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'auth';
class AuthService {
  login = async (email, password) => {
    console.log(api_url);
    const response = await axios.post(api_url + '/login', {
      email,
      password,
    });
    return response.data;
  };
  register = async (username, password) => {
    const response = await axios.post(api_url + '/register', {
      username,
      password,
    });
    return response.data;
  };
  logout = async () => {
    axios.defaults.headers.common['Authorization'] = undefined;
  };
  setToken = (token) => {
    axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
  };
  isAuthenticated = () => {
    console.log(axios.defaults.headers.common['Authorization']);
    return axios.defaults.headers.common['Authorization'] !== undefined;
  };
  getUser = async () => {
    const response = await axios.get(
      import.meta.env.VITE_API_URL + 'user/current'
    );
    console.log(response.data);

    if (response.status === 401) {
      return null;
    }

    return response.data;
  };
}

const authService = new AuthService();
export default authService;
