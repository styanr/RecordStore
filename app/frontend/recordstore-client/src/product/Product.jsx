import { useParams } from 'react-router-dom';
import { useState, useEffect, useLayoutEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import productService from '../hooks/ProductService';

import useAuth from '../hooks/useAuth';

import useCart from '../hooks/useCart';
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
  Textarea,
  useToast,
  IconButton,
  Icon,
  Badge,
} from '@chakra-ui/react';

import { CheckIcon, StarIcon, EditIcon } from '@chakra-ui/icons';

import ReviewList from './ReviewList';
import EditProductForm from './EditProductForm';

import { Link } from 'react-router-dom';

const Product = () => {
  const navigate = useNavigate();
  const location = useLocation();
  const toast = useToast();
  const { isAuthenticated, user } = useAuth();
  const isEmployee =
    user && (user.role === 'employee' || user.role === 'admin');

  const { id } = useParams();

  const [isEditModalOpen, setIsEditModalOpen] = useState(false);

  const handleEditProduct = async (id, updatedData) => {
    try {
      const updatedProduct = await productService.updateProduct(
        id,
        updatedData
      );
      console.log(updatedProduct);

      setProduct(updatedProduct);
      setIsEditModalOpen(false);
      toast({
        title: 'Успіх',
        description: 'Продукт успішно оновлено.',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } catch (error) {
      console.error('Error:', error);
      toast({
        title: 'Помилка',
        description: 'Не вдалося оновити продукт. ' + error.message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  const [review, setReview] = useState({
    content: '',
    rating: 1,
  });

  const isUserUser = user && user.role === 'user';

  useEffect(() => {
    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await productService.getProduct(id);
        setProduct(response);

        const reviewsResponse = await productService.getReviews(id);
        console.log(reviewsResponse);
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

  const [product, setProduct] = useState(undefined);
  const [reviews, setReviews] = useState([]);

  const [otherProducts, setOtherProducts] = useState([]);

  const [isLoading, setIsLoading] = useState(false);

  const { cart, addToCart, updateCart, removeFromCart } = useCart();

  const handleAddToCart = async () => {
    console.log('Add to cart clicked');
    console.log(isAuthenticated);
    if (!isAuthenticated) {
      navigate('/login', { state: { from: location.pathname } });
    }

    try {
      const response = await addToCart(product.id, 1);
      console.log(response);
    } catch (error) {
      console.error('Error:', error);
    }
  };

  const handleCreateReview = async () => {
    try {
      const response = await productService.addReview(product.id, review);
      console.log(response);

      const reviewsResponse = await productService.getReviews(id);
      setReviews(reviewsResponse);

      setReview({ content: '', rating: 1 });

      toast({
        title: 'Успіх',
        description: 'Відгук успішно опубліковано.',
        status: 'success',
        duration: 5000,
        isClosable: true,
      });
    } catch (error) {
      const message = error.response.data.message;
      toast({
        title: 'Помилка',
        description: 'Не вдалося опублікувати відгук. ' + message,
        status: 'error',
        duration: 5000,
        isClosable: true,
      });
    }
  };

  useEffect(() => {
    // this assures the token is set before fetching the product
    if (isAuthenticated === undefined) return;

    const fetchData = async () => {
      setIsLoading(true);
      try {
        const response = await productService.getProduct(id);
        console.log(response);
        setProduct(response);
        console.log(product);
      } catch (error) {
        console.error('Error:', error);
        if (
          error.response &&
          (error.response.status === 404 || error.response.status === 400)
        ) {
          navigate('/');
        }
      } finally {
        setIsLoading(false);
      }
    };

    fetchData();
  }, [isAuthenticated]);

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

  if (product === undefined) {
    return (
      <Box p={8} flexGrow={1}>
        <Text>Loading...</Text>
      </Box>
    );
  }

  return (
    <Box p={8} bg='gray.100' flexGrow={1}>
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
          <HStack spacing={0} align='start' w='full'>
            <Box
              p={8}
              borderRightWidth={1}
              borderRightColor='gray.200'
              position='relative'
            >
              <Image
                src={product.imageUrl}
                alt='product image'
                fallbackSrc='https://via.placeholder.com/150'
                borderRadius='md'
                objectFit='cover'
                boxSize='300px'
              />
              {isEmployee && (
                <IconButton
                  aria-label='Edit product'
                  icon={<Icon as={EditIcon} />}
                  position='absolute'
                  top={2}
                  right={2}
                  bg='blue.500'
                  color='white'
                  _hover={{ bg: 'blue.600' }}
                  onClick={() => setIsEditModalOpen(true)}
                />
              )}
            </Box>
            <VStack spacing={6} align='stretch' flex='1' p={8}>
              <Box>
                <Flex justify='space-between' align='center'>
                  <Heading size='xl' mb={2}>
                    {product.title}
                  </Heading>
                  {product.averageRating ? (
                    <Flex>
                      <StarIcon color='yellow.500' />
                      <Text fontSize='sm' color='yellow.500' ml={2}>
                        {product.averageRating.toFixed(1)}
                      </Text>
                      <Text fontSize='sm' color='gray.500' ml={2}>
                        ({product.totalRatings})
                      </Text>
                    </Flex>
                  ) : (
                    <Text fontSize='sm' color='gray.500'>
                      Немає відгуків
                    </Text>
                  )}
                </Flex>
                {isEmployee && (
                  <Box
                    color='white'
                    fontSize='sm'
                    mb={2}
                    bg='blue.500'
                    py={1}
                    px={3}
                    display={'inline-block'}
                    borderRadius='md'
                  >
                    {product.quantity} шт. на складі
                  </Box>
                )}
                <Box>
                  {product.labelName && (
                    <Badge colorScheme='purple' mr={2}>
                      {product.labelName}
                    </Badge>
                  )}
                </Box>
                <Box>
                  {product.artists
                    ? product.artists.map((artist) => (
                        <Badge
                          key={artist.id}
                          colorScheme='teal'
                          as={Link}
                          to={`/artists/${artist.id}`}
                          mr={2}
                        >
                          {artist.name}
                        </Badge>
                      ))
                    : 'Loading...'}
                </Box>
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
                <Text mb={1} color='orange.500' fontWeight='bold' fontSize='xl'>
                  {product.price} ₴
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
                  {otherProducts.length > 1 ? ( // 1 because the current product is included in the list
                    otherProducts
                      .filter((p) => p.id !== product.id)
                      .map((p) => (
                        <Link
                          to={`/products/${p.id}`}
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
                            <Badge colorScheme='purple' ml={2} px={2} py={1}>
                              {p.labelName}
                            </Badge>
                          </Flex>
                        </Link>
                      ))
                  ) : (
                    <Text color='gray.500'>Інших версій немає</Text>
                  )}
                </VStack>
              </Box>
              <Divider borderColor='gray.300' />
              {isUserUser && (
                <>
                  <Box mt={4}>
                    <Button
                      colorScheme='blue'
                      onClick={(e) => handleAddToCart()}
                      _hover={{ bg: 'blue.600' }}
                      transition='background-color 0.2s ease-in-out'
                    >
                      Додати до кошика
                    </Button>
                  </Box>
                  <Divider borderColor='gray.300' />
                </>
              )}
              <Box>
                <Heading size='md' mb={2}>
                  Відгуки
                </Heading>
                {isAuthenticated ? (
                  isUserUser && (
                    <Box bg='gray.100' p={6} borderRadius='md'>
                      <Heading size='sm' mb={4}>
                        Залишити відгук
                      </Heading>
                      <Box mb={4}>
                        <Heading size='xs' mb={2}>
                          Оцінка
                        </Heading>
                        <HStack>
                          {[1, 2, 3, 4, 5].map((rating) => (
                            <StarIcon
                              key={rating}
                              color={
                                rating <= review.rating
                                  ? 'yellow.500'
                                  : 'gray.300'
                              }
                              boxSize={6}
                              cursor='pointer'
                              onClick={() => setReview({ ...review, rating })}
                            />
                          ))}
                        </HStack>
                      </Box>
                      <Textarea
                        placeholder='Напишіть свій відгук...'
                        rows={4}
                        mb={4}
                        value={review.content || ''}
                        onChange={(e) =>
                          setReview({ ...review, content: e.target.value })
                        }
                      />
                      <Button
                        colorScheme='blue'
                        isDisabled={
                          review.content.trim() === '' || review.rating === 0
                        }
                        onClick={handleCreateReview}
                        rightIcon={<CheckIcon />}
                      >
                        Опублікувати
                      </Button>
                    </Box>
                  )
                ) : (
                  <Text color='gray.500'>
                    Ви маєте{' '}
                    <Text
                      color='blue.500'
                      as={Link}
                      to='/login'
                      _hover={{
                        textDecoration: 'underline',
                      }}
                    >
                      увійти
                    </Text>
                    , щоб залишити відгук.
                  </Text>
                )}
              </Box>
              <ReviewList reviews={reviews} />
            </VStack>
          </HStack>
        )}
      </Flex>
      {isEmployee && (
        <EditProductForm
          isOpen={isEditModalOpen}
          onClose={() => setIsEditModalOpen(false)}
          product={product}
          onSubmit={handleEditProduct}
        />
      )}
    </Box>
  );
};

export default Product;
