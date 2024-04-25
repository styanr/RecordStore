import { useParams } from 'react-router-dom';
import { useState, useEffect, useLayoutEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import productService from '../home/ProductService';
import authService from '../auth/AuthService';
import useCart from '../cart/useCart';
import formatCurrency from '../utils/formatCurrency';

import {
  Box,
  Heading,
  Text,
  Button,
  VStack,
  HStack,
  Divider,
  Image,
  Flex,
} from '@chakra-ui/react';

import { Link } from 'react-router-dom';

const Product = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const { id } = useParams();

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await productService.getProduct(id);
        setProduct(response);

        const reviewsResponse = await productService.getReviews(id);
        setReviews(reviewsResponse);
        console.log(reviews);
      } catch (error) {
        console.error('Error:', error);
      } finally {
        setIsLoading(false);
      }
    };
    fetchData();
  }, [id]);

  useLayoutEffect(() => {
    document.documentElement.scrollTop = 0;
  }, [location.pathname]);

  const [product, setProduct] = useState({});
  const [reviews, setReviews] = useState([]);

  const [otherProducts, setOtherProducts] = useState([]);

  const [isLoading, setIsLoading] = useState(false);

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
    <Box p={8} bg='gray.100' minH='100vh'>
      <Flex
        bg='white'
        boxShadow='md'
        borderRadius='lg'
        overflow='hidden'
        maxW='5xl'
        mx='auto'
        my={8}
      >
        {product && (
          <HStack spacing={0} align='start'>
            <Box
              p={8}
              borderRightWidth={1}
              borderRightColor='gray.200'
              position='relative'
            >
              <Image
                src={product.image_url}
                alt='product image'
                fallbackSrc='https://via.placeholder.com/150'
                borderRadius='md'
                objectFit='cover'
                boxSize='300px'
              />
            </Box>
            <VStack spacing={6} align='stretch' flex='1' p={8}>
              <Box>
                <Heading size='xl' mb={2}>
                  {product.title}
                </Heading>
                <Text color='gray.600' fontSize='sm'>
                  {product.artists
                    ? product.artists.map((artist) => artist.name).join(', ')
                    : 'Loading...'}
                </Text>
                <Flex align='center' mb={1}>
                  {product.format ? (
                    <Text color='gray.600' fontSize='sm' mr={2}>
                      {product.format.name}
                    </Text>
                  ) : (
                    <Text color='gray.400' fontSize='sm' mr={2}>
                      Loading...
                    </Text>
                  )}
                  <Box
                    bg='gray.500'
                    w='6px'
                    h='6px'
                    borderRadius='full'
                    mr={2}
                  />
                  {product.releaseDate ? (
                    <Text color='gray.600' fontSize='sm'>
                      {product.releaseDate.substr(0, 4)}
                    </Text>
                  ) : (
                    <Text color='gray.400' fontSize='sm'>
                      Loading...
                    </Text>
                  )}
                </Flex>
                <Text color='gray.600' fontSize='sm'>
                  {product.genres
                    ? product.genres.map((genre) => genre.name).join(', ')
                    : 'Loading...'}
                </Text>
              </Box>
              <Divider borderColor='gray.300' />
              <Box>
                <Heading size='md' mb={2}>
                  Опис
                </Heading>
                <Text color='gray.700'>
                  {product.description ? product.description : 'Loading...'}
                </Text>
              </Box>
              <Divider borderColor='gray.300' />
              <Box>
                <Heading size='md' mb={2}>
                  Інші версії
                </Heading>
                <VStack align='start' spacing={2}>
                  {otherProducts.length > 0 ? (
                    otherProducts
                      .filter((p) => p.id !== product.id)
                      .map((p) => (
                        <Link
                          to={`/product/${p.id}`}
                          key={p.id}
                          _hover={{ textDecoration: 'none' }}
                        >
                          <Flex align='center'>
                            <Text color='gray.600' mr={2}>
                              {p.format.name}
                            </Text>
                            <Box
                              bg='gray.600'
                              w='6px'
                              h='6px'
                              borderRadius='full'
                              mr={2}
                            />
                            <Text color='gray.600'>
                              {formatCurrency(p.price)}
                            </Text>
                          </Flex>
                        </Link>
                      ))
                  ) : (
                    <Text color='gray.500'>No other versions available</Text>
                  )}
                </VStack>
              </Box>
              <Divider borderColor='gray.300' />
              <Box>
                <Button
                  colorScheme='blue'
                  onClick={(e) => handleAddToCart()}
                  _hover={{ bg: 'blue.600' }}
                  transition='background-color 0.2s ease-in-out'
                >
                  Додати до кошика
                </Button>
              </Box>
            </VStack>
          </HStack>
        )}
      </Flex>
    </Box>
  );
};

export default Product;
