import axios from 'axios';

const api_url = import.meta.env.VITE_API_URL + 'products';
const records_api_url = import.meta.env.VITE_API_URL + 'records';
const cart_api_url = import.meta.env.VITE_API_URL + 'cart';

class ProductService {
  getProducts = async (filterParams) => {
    const response = await axios.get(api_url, { params: filterParams });
    return response.data;
  };

  getProduct = async (id) => {
    const response = await axios.get(api_url + '/' + id);
    return response.data;
  };

  getProductForRecord = async (recordId) => {
    const response = await axios.get(
      records_api_url + '/' + recordId + '/products'
    );
    return response.data;
  };

  getMinMaxPrices = async () => {
    const response = await axios.get(api_url + '/prices');
    return response.data;
  };

  addToCart = async (productId, quantity) => {
    const response = await axios.post(cart_api_url, {
      productId,
      quantity,
    });
    return response.data;
  };

  getReviews = async (productId) => {
    const response = await axios.get(api_url + '/' + productId + '/reviews');
    return response.data;
  }
}

const productService = new ProductService();
export default productService;
