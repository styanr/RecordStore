import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'cart';

class CartService {
  getCart = async () => {
    const response = await axios.get(api_url);
    return response.data;
  };

  addToCart = async (productId, quantity) => {
    const response = await axios.post(api_url, {
      productId,
      quantity,
    });
    return response.data;
  };

  updateCart = async (productId, quantity) => {
    const response = await axios.put(api_url, {
      productId,
      quantity,
    });
    return response.data;
  };

  removeFromCart = async (productId) => {
    const response = await axios.delete(api_url + '/' + productId);
    return response.data;
  };
}

export default CartService;
