import { useParams } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import productService from '../home/ProductService';
import authService from '../auth/AuthService';
import useCart from '../cart/useCart';

import {
  Box,
  Heading,
  Text,
  Button,
  VStack,
  HStack,
  Divider,
  Image,
  Table,
  Thead,
  Tbody,
  Tr,
  Th,
  Td,
  TableContainer,
  Link,
} from '@chakra-ui/react';

const Product = () => {
  const navigate = useNavigate();

  const { id } = useParams();

  const [product, setProduct] = useState({});
  const [otherProducts, setOtherProducts] = useState([]);

  const [isLoading, setIsLoading] = useState(false);
  const location = useLocation();

  const { cart, addToCart, updateCart, removeFromCart } = useCart();

  const handleAddToCart = async () => {
    console.log('Add to cart clicked');
    console.log(authService.isAuthenticated());
    if (!authService.isAuthenticated()) {
      navigate('/login', { state: { from: location.pathname } });
    }

    try {
      const response = await addToCart(product.id, 1);
      console.log(response);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await productService.getProduct(id);
        setProduct(response);
        console.log(product);
      } catch (error) {
        console.error('Error:', error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, []);

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await productService.getProductForRecord(
          product.recordId
        );
        setOtherProducts(response);
        console.log(otherProducts);
      } catch (error) {
        console.error('Error:', error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [product]);

  return (
    <>
      {product && (
        <HStack spacing={4} align='top'>
          <Box p={5}>
            <Image
              src={product.image_url}
              alt='Bleed Like Me Album Cover'
              fallbackSrc='https://via.placeholder.com/150'
            />
          </Box>
          <VStack spacing={4} align='stretch' flex='1' m={5}>
            <Box p={5}>
              <Heading size='lg'>{product.title}</Heading>
              <Text mt={2}>
                {product.artists
                  ? product.artists.map((artist) => artist.name).join(', ')
                  : 'Loading...'}
              </Text>
              <Text mt={2}>
                {product.format ? product.format.name : 'Loading...'}
                {/* a dot separator between format and release year */}
                &#9679;
                {product.releaseDate
                  ? product.releaseDate.substr(0, 4)
                  : 'Loading...'}
              </Text>
              <Text mt={2}>
                {product.genres
                  ? product.genres.map((genre) => genre.name).join(', ')
                  : 'Loading...'}
              </Text>
            </Box>
            <Divider />
            <Box p={5}>
              <Heading size='md'>Опис</Heading>
              <Text>
                {product.description ? product.description : 'Loading...'}
              </Text>
            </Box>
            <Divider />
            <Box p={5}>
              <Heading size='md'>Інші варіанти</Heading>
              <Text>
                {otherProducts.length > 0
                  ? otherProducts
                      .filter((p) => p.id !== product.id)
                      .map((p) => (
                        <Link href={'/product/' + p.id} key={p.id}>
                          {p.format.name} &#9679; {p.price} грн
                        </Link>
                      ))
                  : 'Інших варіантів немає'}
              </Text>
            </Box>
            <Divider />
            <Box p={5}>
              <Button colorScheme='blue' onClick={(e) => handleAddToCart()}>
                Додати в кошик
              </Button>
            </Box>
          </VStack>
        </HStack>
      )}
    </>
  );
};

export default Product;
